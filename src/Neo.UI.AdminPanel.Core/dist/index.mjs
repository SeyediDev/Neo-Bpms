var Fa = Object.defineProperty;
var Ba = (e, t, n) => t in e ? Fa(e, t, { enumerable: !0, configurable: !0, writable: !0, value: n }) : e[t] = n;
var nt = (e, t, n) => Ba(e, typeof t != "symbol" ? t + "" : t, n);
import * as Hn from "react";
import Yt, { createContext as Le, useContext as X, useMemo as he, useRef as de, useCallback as z, useEffect as Ve, useId as Gn, useInsertionEffect as gi, Children as Ua, isValidElement as $a, useLayoutEffect as Ka, useState as Ft, forwardRef as Wa, Fragment as yi, createElement as Ha, Component as Ga } from "react";
import { useDispatch as za, useSelector as Ya, Provider as Xa } from "react-redux";
import { createAsyncThunk as Xt, createSlice as vi, combineReducers as qa, configureStore as Za } from "@reduxjs/toolkit";
import xi from "axios";
import { useLocation as Ti, Navigate as Ja, Link as mt, Outlet as Qa } from "react-router-dom";
import { useQuery as zn, useMutation as Mt, useQueryClient as bi } from "@tanstack/react-query";
var Dt = { exports: {} }, st = {};
/**
 * @license React
 * react-jsx-runtime.production.min.js
 *
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
var Ys;
function el() {
  if (Ys) return st;
  Ys = 1;
  var e = Yt, t = Symbol.for("react.element"), n = Symbol.for("react.fragment"), s = Object.prototype.hasOwnProperty, r = e.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED.ReactCurrentOwner, o = { key: !0, ref: !0, __self: !0, __source: !0 };
  function i(a, l, c) {
    var u, d = {}, f = null, m = null;
    c !== void 0 && (f = "" + c), l.key !== void 0 && (f = "" + l.key), l.ref !== void 0 && (m = l.ref);
    for (u in l) s.call(l, u) && !o.hasOwnProperty(u) && (d[u] = l[u]);
    if (a && a.defaultProps) for (u in l = a.defaultProps, l) d[u] === void 0 && (d[u] = l[u]);
    return { $$typeof: t, type: a, key: f, ref: m, props: d, _owner: r.current };
  }
  return st.Fragment = n, st.jsx = i, st.jsxs = i, st;
}
var rt = {};
/**
 * @license React
 * react-jsx-runtime.development.js
 *
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
var Xs;
function tl() {
  return Xs || (Xs = 1, process.env.NODE_ENV !== "production" && (function() {
    var e = Yt, t = Symbol.for("react.element"), n = Symbol.for("react.portal"), s = Symbol.for("react.fragment"), r = Symbol.for("react.strict_mode"), o = Symbol.for("react.profiler"), i = Symbol.for("react.provider"), a = Symbol.for("react.context"), l = Symbol.for("react.forward_ref"), c = Symbol.for("react.suspense"), u = Symbol.for("react.suspense_list"), d = Symbol.for("react.memo"), f = Symbol.for("react.lazy"), m = Symbol.for("react.offscreen"), g = Symbol.iterator, v = "@@iterator";
    function T(h) {
      if (h === null || typeof h != "object")
        return null;
      var y = g && h[g] || h[v];
      return typeof y == "function" ? y : null;
    }
    var x = e.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED;
    function b(h) {
      {
        for (var y = arguments.length, E = new Array(y > 1 ? y - 1 : 0), R = 1; R < y; R++)
          E[R - 1] = arguments[R];
        w("error", h, E);
      }
    }
    function w(h, y, E) {
      {
        var R = x.ReactDebugCurrentFrame, M = R.getStackAddendum();
        M !== "" && (y += "%s", E = E.concat([M]));
        var I = E.map(function(N) {
          return String(N);
        });
        I.unshift("Warning: " + y), Function.prototype.apply.call(console[h], console, I);
      }
    }
    var P = !1, S = !1, j = !1, O = !1, C = !1, D;
    D = Symbol.for("react.module.reference");
    function V(h) {
      return !!(typeof h == "string" || typeof h == "function" || h === s || h === o || C || h === r || h === c || h === u || O || h === m || P || S || j || typeof h == "object" && h !== null && (h.$$typeof === f || h.$$typeof === d || h.$$typeof === i || h.$$typeof === a || h.$$typeof === l || // This needs to include all possible module reference object
      // types supported by any Flight configuration anywhere since
      // we don't know which Flight build this will end up being used
      // with.
      h.$$typeof === D || h.getModuleId !== void 0));
    }
    function te(h, y, E) {
      var R = h.displayName;
      if (R)
        return R;
      var M = y.displayName || y.name || "";
      return M !== "" ? E + "(" + M + ")" : E;
    }
    function Ee(h) {
      return h.displayName || "Context";
    }
    function fe(h) {
      if (h == null)
        return null;
      if (typeof h.tag == "number" && b("Received an unexpected object in getComponentNameFromType(). This is likely a bug in React. Please file an issue."), typeof h == "function")
        return h.displayName || h.name || null;
      if (typeof h == "string")
        return h;
      switch (h) {
        case s:
          return "Fragment";
        case n:
          return "Portal";
        case o:
          return "Profiler";
        case r:
          return "StrictMode";
        case c:
          return "Suspense";
        case u:
          return "SuspenseList";
      }
      if (typeof h == "object")
        switch (h.$$typeof) {
          case a:
            var y = h;
            return Ee(y) + ".Consumer";
          case i:
            var E = h;
            return Ee(E._context) + ".Provider";
          case l:
            return te(h, h.render, "ForwardRef");
          case d:
            var R = h.displayName || null;
            return R !== null ? R : fe(h.type) || "Memo";
          case f: {
            var M = h, I = M._payload, N = M._init;
            try {
              return fe(N(I));
            } catch {
              return null;
            }
          }
        }
      return null;
    }
    var pe = Object.assign, _e = 0, rn, H, ie, Fe, Be, js, ks;
    function Ns() {
    }
    Ns.__reactDisabledLog = !0;
    function ha() {
      {
        if (_e === 0) {
          rn = console.log, H = console.info, ie = console.warn, Fe = console.error, Be = console.group, js = console.groupCollapsed, ks = console.groupEnd;
          var h = {
            configurable: !0,
            enumerable: !0,
            value: Ns,
            writable: !0
          };
          Object.defineProperties(console, {
            info: h,
            log: h,
            warn: h,
            error: h,
            group: h,
            groupCollapsed: h,
            groupEnd: h
          });
        }
        _e++;
      }
    }
    function fa() {
      {
        if (_e--, _e === 0) {
          var h = {
            configurable: !0,
            enumerable: !0,
            writable: !0
          };
          Object.defineProperties(console, {
            log: pe({}, h, {
              value: rn
            }),
            info: pe({}, h, {
              value: H
            }),
            warn: pe({}, h, {
              value: ie
            }),
            error: pe({}, h, {
              value: Fe
            }),
            group: pe({}, h, {
              value: Be
            }),
            groupCollapsed: pe({}, h, {
              value: js
            }),
            groupEnd: pe({}, h, {
              value: ks
            })
          });
        }
        _e < 0 && b("disabledDepth fell below zero. This is a bug in React. Please file an issue.");
      }
    }
    var on = x.ReactCurrentDispatcher, an;
    function At(h, y, E) {
      {
        if (an === void 0)
          try {
            throw Error();
          } catch (M) {
            var R = M.stack.trim().match(/\n( *(at )?)/);
            an = R && R[1] || "";
          }
        return `
` + an + h;
      }
    }
    var ln = !1, Rt;
    {
      var pa = typeof WeakMap == "function" ? WeakMap : Map;
      Rt = new pa();
    }
    function Vs(h, y) {
      if (!h || ln)
        return "";
      {
        var E = Rt.get(h);
        if (E !== void 0)
          return E;
      }
      var R;
      ln = !0;
      var M = Error.prepareStackTrace;
      Error.prepareStackTrace = void 0;
      var I;
      I = on.current, on.current = null, ha();
      try {
        if (y) {
          var N = function() {
            throw Error();
          };
          if (Object.defineProperty(N.prototype, "props", {
            set: function() {
              throw Error();
            }
          }), typeof Reflect == "object" && Reflect.construct) {
            try {
              Reflect.construct(N, []);
            } catch (Q) {
              R = Q;
            }
            Reflect.construct(h, [], N);
          } else {
            try {
              N.call();
            } catch (Q) {
              R = Q;
            }
            h.call(N.prototype);
          }
        } else {
          try {
            throw Error();
          } catch (Q) {
            R = Q;
          }
          h();
        }
      } catch (Q) {
        if (Q && R && typeof Q.stack == "string") {
          for (var k = Q.stack.split(`
`), J = R.stack.split(`
`), $ = k.length - 1, W = J.length - 1; $ >= 1 && W >= 0 && k[$] !== J[W]; )
            W--;
          for (; $ >= 1 && W >= 0; $--, W--)
            if (k[$] !== J[W]) {
              if ($ !== 1 || W !== 1)
                do
                  if ($--, W--, W < 0 || k[$] !== J[W]) {
                    var oe = `
` + k[$].replace(" at new ", " at ");
                    return h.displayName && oe.includes("<anonymous>") && (oe = oe.replace("<anonymous>", h.displayName)), typeof h == "function" && Rt.set(h, oe), oe;
                  }
                while ($ >= 1 && W >= 0);
              break;
            }
        }
      } finally {
        ln = !1, on.current = I, fa(), Error.prepareStackTrace = M;
      }
      var $e = h ? h.displayName || h.name : "", De = $e ? At($e) : "";
      return typeof h == "function" && Rt.set(h, De), De;
    }
    function ma(h, y, E) {
      return Vs(h, !1);
    }
    function ga(h) {
      var y = h.prototype;
      return !!(y && y.isReactComponent);
    }
    function Pt(h, y, E) {
      if (h == null)
        return "";
      if (typeof h == "function")
        return Vs(h, ga(h));
      if (typeof h == "string")
        return At(h);
      switch (h) {
        case c:
          return At("Suspense");
        case u:
          return At("SuspenseList");
      }
      if (typeof h == "object")
        switch (h.$$typeof) {
          case l:
            return ma(h.render);
          case d:
            return Pt(h.type, y, E);
          case f: {
            var R = h, M = R._payload, I = R._init;
            try {
              return Pt(I(M), y, E);
            } catch {
            }
          }
        }
      return "";
    }
    var tt = Object.prototype.hasOwnProperty, Ms = {}, Os = x.ReactDebugCurrentFrame;
    function Ct(h) {
      if (h) {
        var y = h._owner, E = Pt(h.type, h._source, y ? y.type : null);
        Os.setExtraStackFrame(E);
      } else
        Os.setExtraStackFrame(null);
    }
    function ya(h, y, E, R, M) {
      {
        var I = Function.call.bind(tt);
        for (var N in h)
          if (I(h, N)) {
            var k = void 0;
            try {
              if (typeof h[N] != "function") {
                var J = Error((R || "React class") + ": " + E + " type `" + N + "` is invalid; it must be a function, usually from the `prop-types` package, but received `" + typeof h[N] + "`.This often happens because of typos such as `PropTypes.function` instead of `PropTypes.func`.");
                throw J.name = "Invariant Violation", J;
              }
              k = h[N](y, N, R, E, null, "SECRET_DO_NOT_PASS_THIS_OR_YOU_WILL_BE_FIRED");
            } catch ($) {
              k = $;
            }
            k && !(k instanceof Error) && (Ct(M), b("%s: type specification of %s `%s` is invalid; the type checker function must return `null` or an `Error` but returned a %s. You may have forgotten to pass an argument to the type checker creator (arrayOf, instanceOf, objectOf, oneOf, oneOfType, and shape all require an argument).", R || "React class", E, N, typeof k), Ct(null)), k instanceof Error && !(k.message in Ms) && (Ms[k.message] = !0, Ct(M), b("Failed %s type: %s", E, k.message), Ct(null));
          }
      }
    }
    var va = Array.isArray;
    function cn(h) {
      return va(h);
    }
    function xa(h) {
      {
        var y = typeof Symbol == "function" && Symbol.toStringTag, E = y && h[Symbol.toStringTag] || h.constructor.name || "Object";
        return E;
      }
    }
    function Ta(h) {
      try {
        return Ls(h), !1;
      } catch {
        return !0;
      }
    }
    function Ls(h) {
      return "" + h;
    }
    function Is(h) {
      if (Ta(h))
        return b("The provided key is an unsupported type %s. This value must be coerced to a string before before using it here.", xa(h)), Ls(h);
    }
    var _s = x.ReactCurrentOwner, ba = {
      key: !0,
      ref: !0,
      __self: !0,
      __source: !0
    }, Fs, Bs;
    function Ea(h) {
      if (tt.call(h, "ref")) {
        var y = Object.getOwnPropertyDescriptor(h, "ref").get;
        if (y && y.isReactWarning)
          return !1;
      }
      return h.ref !== void 0;
    }
    function Sa(h) {
      if (tt.call(h, "key")) {
        var y = Object.getOwnPropertyDescriptor(h, "key").get;
        if (y && y.isReactWarning)
          return !1;
      }
      return h.key !== void 0;
    }
    function wa(h, y) {
      typeof h.ref == "string" && _s.current;
    }
    function Aa(h, y) {
      {
        var E = function() {
          Fs || (Fs = !0, b("%s: `key` is not a prop. Trying to access it will result in `undefined` being returned. If you need to access the same value within the child component, you should pass it as a different prop. (https://reactjs.org/link/special-props)", y));
        };
        E.isReactWarning = !0, Object.defineProperty(h, "key", {
          get: E,
          configurable: !0
        });
      }
    }
    function Ra(h, y) {
      {
        var E = function() {
          Bs || (Bs = !0, b("%s: `ref` is not a prop. Trying to access it will result in `undefined` being returned. If you need to access the same value within the child component, you should pass it as a different prop. (https://reactjs.org/link/special-props)", y));
        };
        E.isReactWarning = !0, Object.defineProperty(h, "ref", {
          get: E,
          configurable: !0
        });
      }
    }
    var Pa = function(h, y, E, R, M, I, N) {
      var k = {
        // This tag allows us to uniquely identify this as a React Element
        $$typeof: t,
        // Built-in properties that belong on the element
        type: h,
        key: y,
        ref: E,
        props: N,
        // Record the component responsible for creating this element.
        _owner: I
      };
      return k._store = {}, Object.defineProperty(k._store, "validated", {
        configurable: !1,
        enumerable: !1,
        writable: !0,
        value: !1
      }), Object.defineProperty(k, "_self", {
        configurable: !1,
        enumerable: !1,
        writable: !1,
        value: R
      }), Object.defineProperty(k, "_source", {
        configurable: !1,
        enumerable: !1,
        writable: !1,
        value: M
      }), Object.freeze && (Object.freeze(k.props), Object.freeze(k)), k;
    };
    function Ca(h, y, E, R, M) {
      {
        var I, N = {}, k = null, J = null;
        E !== void 0 && (Is(E), k = "" + E), Sa(y) && (Is(y.key), k = "" + y.key), Ea(y) && (J = y.ref, wa(y, M));
        for (I in y)
          tt.call(y, I) && !ba.hasOwnProperty(I) && (N[I] = y[I]);
        if (h && h.defaultProps) {
          var $ = h.defaultProps;
          for (I in $)
            N[I] === void 0 && (N[I] = $[I]);
        }
        if (k || J) {
          var W = typeof h == "function" ? h.displayName || h.name || "Unknown" : h;
          k && Aa(N, W), J && Ra(N, W);
        }
        return Pa(h, k, J, M, R, _s.current, N);
      }
    }
    var un = x.ReactCurrentOwner, Us = x.ReactDebugCurrentFrame;
    function Ue(h) {
      if (h) {
        var y = h._owner, E = Pt(h.type, h._source, y ? y.type : null);
        Us.setExtraStackFrame(E);
      } else
        Us.setExtraStackFrame(null);
    }
    var dn;
    dn = !1;
    function hn(h) {
      return typeof h == "object" && h !== null && h.$$typeof === t;
    }
    function $s() {
      {
        if (un.current) {
          var h = fe(un.current.type);
          if (h)
            return `

Check the render method of \`` + h + "`.";
        }
        return "";
      }
    }
    function Da(h) {
      return "";
    }
    var Ks = {};
    function ja(h) {
      {
        var y = $s();
        if (!y) {
          var E = typeof h == "string" ? h : h.displayName || h.name;
          E && (y = `

Check the top-level render call using <` + E + ">.");
        }
        return y;
      }
    }
    function Ws(h, y) {
      {
        if (!h._store || h._store.validated || h.key != null)
          return;
        h._store.validated = !0;
        var E = ja(y);
        if (Ks[E])
          return;
        Ks[E] = !0;
        var R = "";
        h && h._owner && h._owner !== un.current && (R = " It was passed a child from " + fe(h._owner.type) + "."), Ue(h), b('Each child in a list should have a unique "key" prop.%s%s See https://reactjs.org/link/warning-keys for more information.', E, R), Ue(null);
      }
    }
    function Hs(h, y) {
      {
        if (typeof h != "object")
          return;
        if (cn(h))
          for (var E = 0; E < h.length; E++) {
            var R = h[E];
            hn(R) && Ws(R, y);
          }
        else if (hn(h))
          h._store && (h._store.validated = !0);
        else if (h) {
          var M = T(h);
          if (typeof M == "function" && M !== h.entries)
            for (var I = M.call(h), N; !(N = I.next()).done; )
              hn(N.value) && Ws(N.value, y);
        }
      }
    }
    function ka(h) {
      {
        var y = h.type;
        if (y == null || typeof y == "string")
          return;
        var E;
        if (typeof y == "function")
          E = y.propTypes;
        else if (typeof y == "object" && (y.$$typeof === l || // Note: Memo only checks outer props here.
        // Inner props are checked in the reconciler.
        y.$$typeof === d))
          E = y.propTypes;
        else
          return;
        if (E) {
          var R = fe(y);
          ya(E, h.props, "prop", R, h);
        } else if (y.PropTypes !== void 0 && !dn) {
          dn = !0;
          var M = fe(y);
          b("Component %s declared `PropTypes` instead of `propTypes`. Did you misspell the property assignment?", M || "Unknown");
        }
        typeof y.getDefaultProps == "function" && !y.getDefaultProps.isReactClassApproved && b("getDefaultProps is only used on classic React.createClass definitions. Use a static property named `defaultProps` instead.");
      }
    }
    function Na(h) {
      {
        for (var y = Object.keys(h.props), E = 0; E < y.length; E++) {
          var R = y[E];
          if (R !== "children" && R !== "key") {
            Ue(h), b("Invalid prop `%s` supplied to `React.Fragment`. React.Fragment can only have `key` and `children` props.", R), Ue(null);
            break;
          }
        }
        h.ref !== null && (Ue(h), b("Invalid attribute `ref` supplied to `React.Fragment`."), Ue(null));
      }
    }
    var Gs = {};
    function zs(h, y, E, R, M, I) {
      {
        var N = V(h);
        if (!N) {
          var k = "";
          (h === void 0 || typeof h == "object" && h !== null && Object.keys(h).length === 0) && (k += " You likely forgot to export your component from the file it's defined in, or you might have mixed up default and named imports.");
          var J = Da();
          J ? k += J : k += $s();
          var $;
          h === null ? $ = "null" : cn(h) ? $ = "array" : h !== void 0 && h.$$typeof === t ? ($ = "<" + (fe(h.type) || "Unknown") + " />", k = " Did you accidentally export a JSX literal instead of a component?") : $ = typeof h, b("React.jsx: type is invalid -- expected a string (for built-in components) or a class/function (for composite components) but got: %s.%s", $, k);
        }
        var W = Ca(h, y, E, M, I);
        if (W == null)
          return W;
        if (N) {
          var oe = y.children;
          if (oe !== void 0)
            if (R)
              if (cn(oe)) {
                for (var $e = 0; $e < oe.length; $e++)
                  Hs(oe[$e], h);
                Object.freeze && Object.freeze(oe);
              } else
                b("React.jsx: Static children should always be an array. You are likely explicitly calling React.jsxs or React.jsxDEV. Use the Babel transform instead.");
            else
              Hs(oe, h);
        }
        if (tt.call(y, "key")) {
          var De = fe(h), Q = Object.keys(y).filter(function(_a) {
            return _a !== "key";
          }), fn = Q.length > 0 ? "{key: someKey, " + Q.join(": ..., ") + ": ...}" : "{key: someKey}";
          if (!Gs[De + fn]) {
            var Ia = Q.length > 0 ? "{" + Q.join(": ..., ") + ": ...}" : "{}";
            b(`A props object containing a "key" prop is being spread into JSX:
  let props = %s;
  <%s {...props} />
React keys must be passed directly to JSX without using spread:
  let props = %s;
  <%s key={someKey} {...props} />`, fn, De, Ia, De), Gs[De + fn] = !0;
          }
        }
        return h === s ? Na(W) : ka(W), W;
      }
    }
    function Va(h, y, E) {
      return zs(h, y, E, !0);
    }
    function Ma(h, y, E) {
      return zs(h, y, E, !1);
    }
    var Oa = Ma, La = Va;
    rt.Fragment = s, rt.jsx = Oa, rt.jsxs = La;
  })()), rt;
}
var qs;
function nl() {
  return qs || (qs = 1, process.env.NODE_ENV === "production" ? Dt.exports = el() : Dt.exports = tl()), Dt.exports;
}
var p = nl();
class ot extends Error {
}
ot.prototype.name = "InvalidTokenError";
function sl(e) {
  return decodeURIComponent(atob(e).replace(/(.)/g, (t, n) => {
    let s = n.charCodeAt(0).toString(16).toUpperCase();
    return s.length < 2 && (s = "0" + s), "%" + s;
  }));
}
function rl(e) {
  let t = e.replace(/-/g, "+").replace(/_/g, "/");
  switch (t.length % 4) {
    case 0:
      break;
    case 2:
      t += "==";
      break;
    case 3:
      t += "=";
      break;
    default:
      throw new Error("base64 string is not of the correct length");
  }
  try {
    return sl(t);
  } catch {
    return atob(t);
  }
}
function il(e, t) {
  if (typeof e != "string")
    throw new ot("Invalid token specified: must be a string");
  t || (t = {});
  const n = t.header === !0 ? 0 : 1, s = e.split(".")[n];
  if (typeof s != "string")
    throw new ot(`Invalid token specified: missing part #${n + 1}`);
  let r;
  try {
    r = rl(s);
  } catch (o) {
    throw new ot(`Invalid token specified: invalid base64 for part #${n + 1} (${o.message})`);
  }
  try {
    return JSON.parse(r);
  } catch (o) {
    throw new ot(`Invalid token specified: invalid json for part #${n + 1} (${o.message})`);
  }
}
const ol = {
  loginEndpoint: "/api/auth/login",
  logoutEndpoint: "/api/auth/logout",
  refreshEndpoint: "/api/auth/refresh",
  userEndpoint: "/api/auth/me",
  tokenKey: "neo_access_token",
  refreshTokenKey: "neo_refresh_token",
  sessionTimeout: 1800 * 1e3,
  // 30 minutes
  autoRefreshBuffer: 300 * 1e3,
  // 5 minutes before expiry
  persistSession: !0,
  redirectAfterLogin: "/",
  redirectAfterLogout: "/login",
  unauthorizedRedirect: "/login"
}, Ei = {
  baseURL: "",
  timeout: 3e4,
  // 30 seconds
  withCredentials: !0,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
    "X-Requested-With": "XMLHttpRequest"
  },
  retryAttempts: 3,
  retryDelay: 1e3
}, Zs = {
  OK: 200,
  CREATED: 201,
  NO_CONTENT: 204,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  CONFLICT: 409,
  UNPROCESSABLE_ENTITY: 422,
  TOO_MANY_REQUESTS: 429,
  INTERNAL_SERVER_ERROR: 500,
  BAD_GATEWAY: 502,
  SERVICE_UNAVAILABLE: 503,
  GATEWAY_TIMEOUT: 504
}, pn = {
  NETWORK_ERROR: "NETWORK_ERROR",
  TIMEOUT: "TIMEOUT",
  UNAUTHORIZED: "UNAUTHORIZED",
  FORBIDDEN: "FORBIDDEN",
  NOT_FOUND: "NOT_FOUND",
  VALIDATION_ERROR: "VALIDATION_ERROR",
  SERVER_ERROR: "SERVER_ERROR",
  UNKNOWN_ERROR: "UNKNOWN_ERROR",
  TOKEN_EXPIRED: "TOKEN_EXPIRED",
  INVALID_TOKEN: "INVALID_TOKEN",
  SESSION_EXPIRED: "SESSION_EXPIRED"
}, L = {
  ACCESS_TOKEN: "neo_access_token",
  REFRESH_TOKEN: "neo_refresh_token",
  USER: "neo_user",
  THEME: "neo_theme",
  LANGUAGE: "neo_language",
  SIDEBAR_COLLAPSED: "neo_sidebar_collapsed",
  TABLE_SETTINGS: "neo_table_settings"
}, Y = {
  AUTH_STATE_CHANGED: "neo:auth:state_changed",
  TOKEN_REFRESHED: "neo:auth:token_refreshed",
  SESSION_EXPIRED: "neo:auth:session_expired",
  UNAUTHORIZED: "neo:auth:unauthorized",
  PERMISSION_DENIED: "neo:auth:permission_denied",
  NETWORK_ERROR: "neo:network:error",
  SERVER_ERROR: "neo:server:error"
}, Af = {
  USER: ["user"],
  USERS: ["users"],
  ROLES: ["roles"],
  PERMISSIONS: ["permissions"],
  SESSIONS: ["sessions"]
}, Rf = {
  PAGE: 1,
  PAGE_SIZE: 20,
  PAGE_SIZE_OPTIONS: [10, 20, 50, 100],
  MAX_PAGE_SIZE: 100
}, al = {
  user: null,
  accessToken: null,
  refreshToken: null,
  expiresAt: null,
  isAuthenticated: !1,
  isLoading: !1,
  error: null,
  lastActivity: null
}, ll = () => {
  try {
    const e = localStorage.getItem(L.ACCESS_TOKEN), t = localStorage.getItem(L.REFRESH_TOKEN), n = localStorage.getItem(L.USER);
    if (!e || !n)
      return {};
    const r = il(e).exp * 1e3;
    return Date.now() >= r ? (localStorage.removeItem(L.ACCESS_TOKEN), localStorage.removeItem(L.REFRESH_TOKEN), localStorage.removeItem(L.USER), {}) : {
      user: JSON.parse(n),
      accessToken: e,
      refreshToken: t,
      expiresAt: r,
      isAuthenticated: !0,
      lastActivity: Date.now()
    };
  } catch {
    return {};
  }
}, ce = (e) => {
  e.accessToken && e.user ? (localStorage.setItem(L.ACCESS_TOKEN, e.accessToken), e.refreshToken && localStorage.setItem(L.REFRESH_TOKEN, e.refreshToken), localStorage.setItem(L.USER, JSON.stringify(e.user))) : (localStorage.removeItem(L.ACCESS_TOKEN), localStorage.removeItem(L.REFRESH_TOKEN), localStorage.removeItem(L.USER));
}, Se = (e, t) => {
  window.dispatchEvent(new CustomEvent(e, { detail: t }));
}, ut = Xt("auth/login", async ({ credentials: e, apiClient: t }, { rejectWithValue: n }) => {
  try {
    return (await t.post("/api/auth/login", e)).data;
  } catch (s) {
    const r = s instanceof Error ? s.message : "Login failed";
    return n(r);
  }
}), Ot = Xt("auth/logout", async (e) => {
  try {
    e != null && e.apiClient && await e.apiClient.post("/api/auth/logout");
  } catch (t) {
    console.error("Logout API error:", t);
  }
}), Bt = Xt("auth/refreshToken", async ({ apiClient: e }, { getState: t, rejectWithValue: n }) => {
  try {
    const r = t().auth.refreshToken;
    return r ? (await e.post("/api/auth/refresh", { refreshToken: r })).data : n("No refresh token available");
  } catch (s) {
    const r = s instanceof Error ? s.message : "Token refresh failed";
    return n(r);
  }
}), mn = Xt("auth/fetchUser", async ({ apiClient: e }, { rejectWithValue: t }) => {
  try {
    return (await e.get("/api/auth/me")).data;
  } catch (n) {
    const s = n instanceof Error ? n.message : "Failed to fetch user";
    return t(s);
  }
}), Si = vi({
  name: "auth",
  initialState: { ...al, ...ll() },
  reducers: {
    /**
     * Set user manually
     */
    setUser: (e, t) => {
      e.user = t.payload, e.isAuthenticated = !!t.payload, ce(e), Se(Y.AUTH_STATE_CHANGED, { user: t.payload });
    },
    /**
     * Update user partially
     */
    updateUser: (e, t) => {
      e.user && (e.user = { ...e.user, ...t.payload }, ce(e));
    },
    /**
     * Set tokens
     */
    setTokens: (e, t) => {
      const { accessToken: n, refreshToken: s, expiresIn: r } = t.payload;
      e.accessToken = n, s && (e.refreshToken = s), e.expiresAt = Date.now() + r * 1e3, ce(e), Se(Y.TOKEN_REFRESHED);
    },
    /**
     * Update last activity timestamp
     */
    updateLastActivity: (e) => {
      e.lastActivity = Date.now();
    },
    /**
     * Clear auth state (for logout)
     */
    clearAuth: (e) => {
      e.user = null, e.accessToken = null, e.refreshToken = null, e.expiresAt = null, e.isAuthenticated = !1, e.error = null, e.lastActivity = null, ce(e), Se(Y.AUTH_STATE_CHANGED, { user: null });
    },
    /**
     * Set error
     */
    setError: (e, t) => {
      e.error = t.payload;
    },
    /**
     * Clear error
     */
    clearError: (e) => {
      e.error = null;
    },
    /**
     * Handle session expiry
     */
    sessionExpired: (e) => {
      e.user = null, e.accessToken = null, e.refreshToken = null, e.expiresAt = null, e.isAuthenticated = !1, e.error = "Session expired. Please login again.", ce(e), Se(Y.SESSION_EXPIRED);
    }
  },
  extraReducers: (e) => {
    e.addCase(ut.pending, (t) => {
      t.isLoading = !0, t.error = null;
    }).addCase(ut.fulfilled, (t, n) => {
      const { user: s, accessToken: r, refreshToken: o, expiresIn: i } = n.payload;
      t.user = s, t.accessToken = r, t.refreshToken = o, t.expiresAt = Date.now() + i * 1e3, t.isAuthenticated = !0, t.isLoading = !1, t.error = null, t.lastActivity = Date.now(), ce(t), Se(Y.AUTH_STATE_CHANGED, { user: s });
    }).addCase(ut.rejected, (t, n) => {
      t.isLoading = !1, t.error = n.payload || "Login failed", t.isAuthenticated = !1;
    }), e.addCase(Ot.pending, (t) => {
      t.isLoading = !0;
    }).addCase(Ot.fulfilled, (t) => {
      t.user = null, t.accessToken = null, t.refreshToken = null, t.expiresAt = null, t.isAuthenticated = !1, t.isLoading = !1, t.error = null, t.lastActivity = null, ce(t), Se(Y.AUTH_STATE_CHANGED, { user: null });
    }).addCase(Ot.rejected, (t) => {
      t.user = null, t.accessToken = null, t.refreshToken = null, t.expiresAt = null, t.isAuthenticated = !1, t.isLoading = !1, ce(t);
    }), e.addCase(Bt.fulfilled, (t, n) => {
      const { accessToken: s, refreshToken: r, expiresIn: o } = n.payload;
      t.accessToken = s, t.refreshToken = r, t.expiresAt = Date.now() + o * 1e3, ce(t), Se(Y.TOKEN_REFRESHED);
    }).addCase(Bt.rejected, (t, n) => {
      t.user = null, t.accessToken = null, t.refreshToken = null, t.expiresAt = null, t.isAuthenticated = !1, t.error = n.payload || "Session expired", ce(t), Se(Y.SESSION_EXPIRED);
    }), e.addCase(mn.pending, (t) => {
      t.isLoading = !0;
    }).addCase(mn.fulfilled, (t, n) => {
      t.user = n.payload, t.isLoading = !1, ce(t);
    }).addCase(mn.rejected, (t, n) => {
      t.isLoading = !1, t.error = n.payload || "Failed to fetch user";
    });
  }
}), {
  setUser: Pf,
  updateUser: cl,
  setTokens: Cf,
  updateLastActivity: ul,
  clearAuth: dl,
  setError: Df,
  clearError: jf,
  sessionExpired: kf
} = Si.actions, Yn = (e) => e.auth.user, wi = (e) => e.auth.isAuthenticated, Ai = (e) => e.auth.isLoading, Ri = (e) => e.auth.error, Pi = (e) => e.auth.accessToken, Ci = (e) => e.auth.expiresAt, Nf = (e) => (t) => {
  const n = t.auth.user;
  return n ? n.isAdmin ? !0 : n.permissions.some((s) => s.id === e || s.name === e) : !1;
}, Vf = (e) => (t) => {
  const n = t.auth.user;
  return n ? n.isAdmin ? !0 : n.roles.includes(e) : !1;
}, hl = Si.reducer, Di = {
  mode: "light",
  primaryColor: "#0ea5e9",
  borderRadius: "md",
  fontFamily: "Vazirmatn",
  direction: "rtl"
}, fl = () => {
  try {
    const e = localStorage.getItem(L.THEME), t = localStorage.getItem(L.LANGUAGE), n = localStorage.getItem(L.SIDEBAR_COLLAPSED);
    return {
      theme: e ? JSON.parse(e) : Di,
      language: t || "fa",
      sidebarCollapsed: n === "true"
    };
  } catch {
    return {};
  }
}, pl = {
  theme: Di,
  language: "fa",
  sidebarCollapsed: !1,
  sidebarMobileOpen: !1,
  notifications: [],
  isLoading: !1,
  loadingMessage: null,
  breadcrumbs: [],
  pageTitle: null,
  ...fl()
};
let ml = 0;
const gl = () => `notification_${++ml}_${Date.now()}`, ji = vi({
  name: "ui",
  initialState: pl,
  reducers: {
    /**
     * Set theme
     */
    setTheme: (e, t) => {
      e.theme = { ...e.theme, ...t.payload }, localStorage.setItem(L.THEME, JSON.stringify(e.theme));
      const n = document.documentElement;
      n.setAttribute("data-theme", e.theme.mode), n.setAttribute("dir", e.theme.direction), n.style.setProperty("--primary-color", e.theme.primaryColor);
    },
    /**
     * Toggle theme mode
     */
    toggleThemeMode: (e) => {
      e.theme.mode = e.theme.mode === "light" ? "dark" : "light", localStorage.setItem(L.THEME, JSON.stringify(e.theme)), document.documentElement.setAttribute("data-theme", e.theme.mode);
    },
    /**
     * Set language
     */
    setLanguage: (e, t) => {
      e.language = t.payload, localStorage.setItem(L.LANGUAGE, t.payload);
      const n = ["fa", "ar", "he"].includes(t.payload);
      e.theme.direction = n ? "rtl" : "ltr", document.documentElement.setAttribute("dir", e.theme.direction), document.documentElement.setAttribute("lang", t.payload);
    },
    /**
     * Toggle sidebar collapsed state
     */
    toggleSidebar: (e) => {
      e.sidebarCollapsed = !e.sidebarCollapsed, localStorage.setItem(L.SIDEBAR_COLLAPSED, String(e.sidebarCollapsed));
    },
    /**
     * Set sidebar collapsed state
     */
    setSidebarCollapsed: (e, t) => {
      e.sidebarCollapsed = t.payload, localStorage.setItem(L.SIDEBAR_COLLAPSED, String(t.payload));
    },
    /**
     * Toggle mobile sidebar
     */
    toggleMobileSidebar: (e) => {
      e.sidebarMobileOpen = !e.sidebarMobileOpen;
    },
    /**
     * Set mobile sidebar state
     */
    setMobileSidebarOpen: (e, t) => {
      e.sidebarMobileOpen = t.payload;
    },
    /**
     * Add notification
     */
    addNotification: (e, t) => {
      const n = {
        ...t.payload,
        id: gl()
      };
      e.notifications.push(n), n.duration && n.duration > 0 && setTimeout(() => {
      }, n.duration);
    },
    /**
     * Remove notification
     */
    removeNotification: (e, t) => {
      e.notifications = e.notifications.filter(
        (n) => n.id !== t.payload
      );
    },
    /**
     * Clear all notifications
     */
    clearNotifications: (e) => {
      e.notifications = [];
    },
    /**
     * Show loading overlay
     */
    showLoading: (e, t) => {
      e.isLoading = !0, e.loadingMessage = t.payload || null;
    },
    /**
     * Hide loading overlay
     */
    hideLoading: (e) => {
      e.isLoading = !1, e.loadingMessage = null;
    },
    /**
     * Set breadcrumbs
     */
    setBreadcrumbs: (e, t) => {
      e.breadcrumbs = t.payload;
    },
    /**
     * Set page title
     */
    setPageTitle: (e, t) => {
      e.pageTitle = t.payload, t.payload && (document.title = `${t.payload} | Neo BPMS`);
    }
  }
}), {
  setTheme: Mf,
  toggleThemeMode: yl,
  setLanguage: Of,
  toggleSidebar: vl,
  setSidebarCollapsed: Lf,
  toggleMobileSidebar: xl,
  setMobileSidebarOpen: Tl,
  addNotification: If,
  removeNotification: bl,
  clearNotifications: _f,
  showLoading: Ff,
  hideLoading: Bf,
  setBreadcrumbs: Uf,
  setPageTitle: $f
} = ji.actions, ki = (e) => e.ui.theme, Kf = (e) => e.ui.language, Ni = (e) => e.ui.sidebarCollapsed, Vi = (e) => e.ui.sidebarMobileOpen, El = (e) => e.ui.notifications, Wf = (e) => e.ui.isLoading, Hf = (e) => e.ui.loadingMessage, Sl = (e) => e.ui.breadcrumbs, wl = (e) => e.ui.pageTitle, Al = ji.reducer, Rl = qa({
  auth: hl,
  ui: Al
}), Pl = (e) => (t) => (n) => {
  if (process.env.NODE_ENV === "development") {
    console.group(n.type || "unknown"), console.log("Previous State:", e.getState()), console.log("Action:", n);
    const r = t(n);
    return console.log("Next State:", e.getState()), console.groupEnd(), r;
  }
  return t(n);
}, Js = () => (e) => (t) => {
  try {
    return e(t);
  } catch (n) {
    throw console.error("Redux error:", n), n;
  }
}, Cl = (e) => Za({
  reducer: Rl,
  preloadedState: e,
  middleware: (t) => {
    const n = t({
      serializableCheck: {
        // Ignore these paths in the state for serialization check
        ignoredPaths: ["auth.user.metadata"],
        ignoredActions: ["auth/setUser"]
      },
      thunk: {
        extraArgument: void 0
      }
    });
    return process.env.NODE_ENV === "development" ? n.concat(Pl, Js) : n.concat(Js);
  },
  devTools: process.env.NODE_ENV === "development"
}), Xn = Cl(), qt = () => za(), U = Ya, Gf = () => Xn.getState(), zf = Xn.dispatch;
function Dl(e = {}) {
  const t = { ...Ei, ...e };
  return xi.create({
    baseURL: t.baseURL,
    timeout: t.timeout,
    withCredentials: t.withCredentials,
    headers: t.headers
  });
}
class jl {
  constructor(t = {}) {
    nt(this, "instance");
    nt(this, "config");
    nt(this, "isRefreshing", !1);
    nt(this, "refreshSubscribers", []);
    this.config = { ...Ei, ...t }, this.instance = Dl(t), this.setupInterceptors();
  }
  /**
   * Get the underlying Axios instance
   */
  getAxiosInstance() {
    return this.instance;
  }
  /**
   * Setup request and response interceptors
   */
  setupInterceptors() {
    this.instance.interceptors.request.use(
      this.handleRequest.bind(this),
      this.handleRequestError.bind(this)
    ), this.instance.interceptors.response.use(
      this.handleResponse.bind(this),
      this.handleResponseError.bind(this)
    );
  }
  /**
   * Handle outgoing requests
   */
  handleRequest(t) {
    var s;
    const n = this.getAccessToken();
    return n && (t.headers.Authorization = `Bearer ${n}`), t.headers["X-Request-ID"] = this.generateRequestId(), t.headers["X-Request-Time"] = (/* @__PURE__ */ new Date()).toISOString(), process.env.NODE_ENV === "development" && console.log(`[API] ${(s = t.method) == null ? void 0 : s.toUpperCase()} ${t.url}`, {
      params: t.params,
      data: t.data
    }), t;
  }
  /**
   * Handle request errors
   */
  handleRequestError(t) {
    return console.error("[API] Request error:", t), Promise.reject(t);
  }
  /**
   * Handle incoming responses
   */
  handleResponse(t) {
    return process.env.NODE_ENV === "development" && console.log(`[API] Response ${t.status}:`, t.data), t;
  }
  /**
   * Handle response errors
   */
  async handleResponseError(t) {
    var o, i, a, l, c, u;
    const n = t.config;
    if (!t.response)
      return (i = (o = this.config).onNetworkError) == null || i.call(o, t), this.dispatchEvent(Y.NETWORK_ERROR, t), Promise.reject(this.createApiError(pn.NETWORK_ERROR, "Network error"));
    const { status: s, data: r } = t.response;
    if (s === Zs.UNAUTHORIZED && !n._retry) {
      if (this.isRefreshing)
        return new Promise((d) => {
          this.refreshSubscribers.push((f) => {
            n.headers.Authorization = `Bearer ${f}`, d(this.instance(n));
          });
        });
      n._retry = !0, this.isRefreshing = !0;
      try {
        const d = await this.refreshToken();
        if (d)
          return this.notifyRefreshSubscribers(d), n.headers.Authorization = `Bearer ${d}`, this.instance(n);
      } catch (d) {
        return this.clearAuth(), (l = (a = this.config).onUnauthorized) == null || l.call(a), this.dispatchEvent(Y.UNAUTHORIZED), Promise.reject(d);
      } finally {
        this.isRefreshing = !1, this.refreshSubscribers = [];
      }
    }
    if (s === Zs.FORBIDDEN && this.dispatchEvent(Y.PERMISSION_DENIED), s >= 500) {
      const d = this.extractApiError(r);
      (u = (c = this.config).onServerError) == null || u.call(c, d), this.dispatchEvent(Y.SERVER_ERROR, d);
    }
    return process.env.NODE_ENV === "development" && console.error(`[API] Error ${s}:`, r), Promise.reject(this.extractApiError(r, s));
  }
  /**
   * Refresh the access token
   */
  async refreshToken() {
    const t = localStorage.getItem(L.REFRESH_TOKEN);
    if (!t) return null;
    try {
      const n = await xi.post(
        `${this.config.baseURL}/api/auth/refresh`,
        { refreshToken: t },
        { withCredentials: !0 }
      ), { accessToken: s, refreshToken: r } = n.data;
      return localStorage.setItem(L.ACCESS_TOKEN, s), r && localStorage.setItem(L.REFRESH_TOKEN, r), this.dispatchEvent(Y.TOKEN_REFRESHED, { accessToken: s }), s;
    } catch {
      return null;
    }
  }
  /**
   * Notify all subscribers waiting for token refresh
   */
  notifyRefreshSubscribers(t) {
    this.refreshSubscribers.forEach((n) => n(t));
  }
  /**
   * Get access token from storage
   */
  getAccessToken() {
    return localStorage.getItem(L.ACCESS_TOKEN);
  }
  /**
   * Clear authentication data
   */
  clearAuth() {
    localStorage.removeItem(L.ACCESS_TOKEN), localStorage.removeItem(L.REFRESH_TOKEN), localStorage.removeItem(L.USER);
  }
  /**
   * Generate unique request ID
   */
  generateRequestId() {
    return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }
  /**
   * Create standardized API error
   */
  createApiError(t, n, s) {
    return { code: t, message: n, field: s };
  }
  /**
   * Extract API error from response data
   */
  extractApiError(t, n) {
    if (typeof t == "object" && t !== null) {
      const s = t;
      return {
        code: s.code || String(n) || pn.UNKNOWN_ERROR,
        message: s.message || "An error occurred",
        field: s.field,
        details: s.details
      };
    }
    return this.createApiError(pn.UNKNOWN_ERROR, String(t));
  }
  /**
   * Dispatch custom event
   */
  dispatchEvent(t, n) {
    window.dispatchEvent(new CustomEvent(t, { detail: n }));
  }
  // ==================== Public API Methods ====================
  /**
   * GET request
   */
  async get(t, n) {
    return (await this.instance.get(t, n)).data;
  }
  /**
   * POST request
   */
  async post(t, n, s) {
    return (await this.instance.post(t, n, s)).data;
  }
  /**
   * PUT request
   */
  async put(t, n, s) {
    return (await this.instance.put(t, n, s)).data;
  }
  /**
   * PATCH request
   */
  async patch(t, n, s) {
    return (await this.instance.patch(t, n, s)).data;
  }
  /**
   * DELETE request
   */
  async delete(t, n) {
    return (await this.instance.delete(t, n)).data;
  }
  /**
   * Upload file
   */
  async uploadFile(t, n, s = "file", r, o) {
    const i = new FormData();
    return i.append(s, n), r && Object.entries(r).forEach(([l, c]) => {
      i.append(l, String(c));
    }), (await this.instance.post(t, i, {
      headers: {
        "Content-Type": "multipart/form-data"
      },
      onUploadProgress: (l) => {
        if (l.total && o) {
          const c = Math.round(l.loaded * 100 / l.total);
          o(c);
        }
      }
    })).data;
  }
  /**
   * Download file
   */
  async downloadFile(t, n) {
    const s = await this.instance.get(t, {
      responseType: "blob"
    }), r = new Blob([s.data]), o = window.URL.createObjectURL(r), i = document.createElement("a");
    i.href = o, i.download = n || this.extractFilename(s) || "download", document.body.appendChild(i), i.click(), document.body.removeChild(i), window.URL.revokeObjectURL(o);
  }
  /**
   * Extract filename from response headers
   */
  extractFilename(t) {
    const n = t.headers["content-disposition"];
    if (n) {
      const s = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(n);
      if (s != null && s[1])
        return s[1].replace(/['"]/g, "");
    }
    return null;
  }
  /**
   * Set authorization token manually
   */
  setAuthToken(t) {
    localStorage.setItem(L.ACCESS_TOKEN, t), this.instance.defaults.headers.common.Authorization = `Bearer ${t}`;
  }
  /**
   * Remove authorization token
   */
  removeAuthToken() {
    this.clearAuth(), delete this.instance.defaults.headers.common.Authorization;
  }
  /**
   * Update base URL
   */
  setBaseURL(t) {
    this.config.baseURL = t, this.instance.defaults.baseURL = t;
  }
}
const ne = new jl(), Mi = Le(null);
function kl({
  children: e,
  config: t,
  onAuthStateChange: n,
  onSessionExpired: s,
  onUnauthorized: r
}) {
  const o = qt(), i = U(Yn), a = U(wi), l = U(Ai), c = U(Ri), u = U(Pi), d = U(Ci), f = he(() => ({ ...ol, ...t }), [t]), m = de(null), g = de(null), v = de(a), T = z(async (D) => {
    const V = await o(ut({ credentials: D, apiClient: ne }));
    if (ut.fulfilled.match(V))
      return V.payload;
    throw new Error(V.payload || "Login failed");
  }, [o]), x = z(async () => {
    m.current && (clearTimeout(m.current), m.current = null), await o(Ot(void 0));
  }, [o]), b = z(async () => {
    const D = await o(Bt({ apiClient: ne }));
    Bt.rejected.match(D) && (s == null || s());
  }, [o, s]), w = z((D) => i ? i.isAdmin ? !0 : (Array.isArray(D) ? D : [D]).some(
    (te) => i.permissions.some((Ee) => Ee.id === te || Ee.name === te)
  ) : !1, [i]), P = z((D) => i ? i.isAdmin ? !0 : (Array.isArray(D) ? D : [D]).some((te) => i.roles.includes(te)) : !1, [i]), S = z((D) => {
    o(cl(D));
  }, [o]), j = z(() => {
    if (!d || !u) return;
    m.current && clearTimeout(m.current);
    const D = d - f.autoRefreshBuffer, V = Math.max(0, D - Date.now());
    V > 0 ? m.current = setTimeout(() => {
      b();
    }, V) : b();
  }, [d, u, f.autoRefreshBuffer, b]), O = z(() => {
    o(ul()), g.current && clearTimeout(g.current), a && f.sessionTimeout > 0 && (g.current = setTimeout(() => {
      o(dl()), s == null || s();
    }, f.sessionTimeout));
  }, [o, a, f.sessionTimeout, s]);
  Ve(() => (a && u && j(), () => {
    m.current && clearTimeout(m.current);
  }), [a, u, j]), Ve(() => {
    if (a) {
      const D = ["mousedown", "keydown", "scroll", "touchstart"];
      return D.forEach((V) => {
        window.addEventListener(V, O, { passive: !0 });
      }), O(), () => {
        D.forEach((V) => {
          window.removeEventListener(V, O);
        }), g.current && clearTimeout(g.current);
      };
    }
  }, [a, O]), Ve(() => {
    v.current !== a && (v.current = a, n == null || n(a, i));
  }, [a, i, n]), Ve(() => {
    const D = () => {
      r == null || r();
    }, V = () => {
      s == null || s();
    };
    return window.addEventListener(Y.UNAUTHORIZED, D), window.addEventListener(Y.SESSION_EXPIRED, V), () => {
      window.removeEventListener(Y.UNAUTHORIZED, D), window.removeEventListener(Y.SESSION_EXPIRED, V);
    };
  }, [r, s]);
  const C = he(() => ({
    user: i,
    isAuthenticated: a,
    isLoading: l,
    error: c,
    login: T,
    logout: x,
    refreshToken: b,
    hasPermission: w,
    hasRole: P,
    updateUser: S
  }), [
    i,
    a,
    l,
    c,
    T,
    x,
    b,
    w,
    P,
    S
  ]);
  return /* @__PURE__ */ p.jsx(Mi.Provider, { value: C, children: e });
}
function Yf(e) {
  return /* @__PURE__ */ p.jsx(Xa, { store: Xn, children: /* @__PURE__ */ p.jsx(kl, { ...e }) });
}
function Nl() {
  const e = X(Mi);
  if (!e)
    throw new Error("useAuthContext must be used within an AuthProvider");
  return e;
}
function ve() {
  const e = Nl(), t = U(Yn), n = U(Pi), s = U(Ci), r = z((c) => c.some((u) => e.hasPermission(u)), [e]), o = z((c) => c.every((u) => e.hasPermission(u)), [e]), i = z((c) => c.some((u) => e.hasRole(u)), [e]), a = z((c) => c.every((u) => e.hasRole(u)), [e]), l = he(() => ({
    isAdmin: (t == null ? void 0 : t.isAdmin) ?? !1,
    displayName: (t == null ? void 0 : t.displayName) ?? (t == null ? void 0 : t.username) ?? "",
    permissions: (t == null ? void 0 : t.permissions) ?? [],
    roles: (t == null ? void 0 : t.roles) ?? []
  }), [t]);
  return {
    // State from context
    user: e.user,
    isAuthenticated: e.isAuthenticated,
    isLoading: e.isLoading,
    error: e.error,
    accessToken: n,
    expiresAt: s,
    // Actions from context
    login: e.login,
    logout: e.logout,
    refreshToken: e.refreshToken,
    updateUser: e.updateUser,
    // Permission helpers
    hasPermission: e.hasPermission,
    hasRole: e.hasRole,
    hasAnyPermission: r,
    hasAllPermissions: o,
    hasAnyRole: i,
    hasAllRoles: a,
    // User helpers
    ...l
  };
}
function Xf() {
  return U(wi);
}
function qf() {
  return U(Yn);
}
function Zf() {
  return U(Ai);
}
function Jf() {
  return U(Ri);
}
function Qf(e) {
  const { hasPermission: t } = ve();
  return t(e);
}
function ep(e) {
  const { hasRole: t } = ve();
  return t(e);
}
function tp() {
  const e = ve();
  if (!e.isAuthenticated && !e.isLoading)
    throw new Error("User is not authenticated");
  return e;
}
function Vl() {
  return /* @__PURE__ */ p.jsx("div", { className: "flex items-center justify-center min-h-screen bg-slate-100 dark:bg-slate-900", children: /* @__PURE__ */ p.jsxs("div", { className: "flex flex-col items-center gap-4", children: [
    /* @__PURE__ */ p.jsx("div", { className: "w-12 h-12 border-4 border-primary-500 border-t-transparent rounded-full animate-spin" }),
    /* @__PURE__ */ p.jsx("p", { className: "text-slate-600 dark:text-slate-400", children: "در حال بارگذاری..." })
  ] }) });
}
function Ml() {
  return /* @__PURE__ */ p.jsx("div", { className: "flex items-center justify-center min-h-screen bg-slate-100 dark:bg-slate-900", children: /* @__PURE__ */ p.jsxs("div", { className: "text-center p-8 bg-white dark:bg-slate-800 rounded-xl shadow-lg max-w-md", children: [
    /* @__PURE__ */ p.jsx("div", { className: "w-16 h-16 mx-auto mb-4 bg-red-100 dark:bg-red-900/30 rounded-full flex items-center justify-center", children: /* @__PURE__ */ p.jsx("svg", { className: "w-8 h-8 text-red-600 dark:text-red-400", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" }) }) }),
    /* @__PURE__ */ p.jsx("h2", { className: "text-xl font-bold text-slate-800 dark:text-white mb-2", children: "دسترسی غیرمجاز" }),
    /* @__PURE__ */ p.jsx("p", { className: "text-slate-600 dark:text-slate-400 mb-4", children: "شما مجوز دسترسی به این صفحه را ندارید." }),
    /* @__PURE__ */ p.jsx(
      "a",
      {
        href: "/",
        className: "inline-block px-4 py-2 bg-primary-500 text-white rounded-lg hover:bg-primary-600 transition-colors",
        children: "بازگشت به صفحه اصلی"
      }
    )
  ] }) });
}
function Ol({
  children: e,
  requiredPermissions: t = [],
  requiredRoles: n = [],
  requireAll: s = !1,
  fallback: r,
  redirectTo: o = "/login"
}) {
  const {
    isAuthenticated: i,
    isLoading: a,
    hasAllPermissions: l,
    hasAnyPermission: c,
    hasAllRoles: u,
    hasAnyRole: d
  } = ve(), f = Ti(), m = he(() => {
    if (t.length === 0 && n.length === 0)
      return !0;
    const g = t.length === 0 || (s ? l(t) : c(t)), v = n.length === 0 || (s ? u(n) : d(n));
    return t.length > 0 && n.length > 0 ? s ? g && v : g || v : g && v;
  }, [
    t,
    n,
    s,
    l,
    c,
    u,
    d
  ]);
  return a ? r ? /* @__PURE__ */ p.jsx(p.Fragment, { children: r }) : /* @__PURE__ */ p.jsx(Vl, {}) : i ? m ? /* @__PURE__ */ p.jsx(p.Fragment, { children: e }) : r ? /* @__PURE__ */ p.jsx(p.Fragment, { children: r }) : /* @__PURE__ */ p.jsx(Ml, {}) : /* @__PURE__ */ p.jsx(
    Ja,
    {
      to: o,
      state: { from: f.pathname + f.search },
      replace: !0
    }
  );
}
function np(e, t = {}) {
  return function(s) {
    return /* @__PURE__ */ p.jsx(Ol, { ...t, children: /* @__PURE__ */ p.jsx(e, { ...s }) });
  };
}
function sp({
  permission: e,
  children: t,
  fallback: n = null
}) {
  const { hasPermission: s, isAuthenticated: r } = ve();
  return r ? (Array.isArray(e) ? e : [e]).some((a) => s(a)) ? /* @__PURE__ */ p.jsx(p.Fragment, { children: t }) : /* @__PURE__ */ p.jsx(p.Fragment, { children: n }) : /* @__PURE__ */ p.jsx(p.Fragment, { children: n });
}
function rp({
  role: e,
  children: t,
  fallback: n = null
}) {
  const { hasRole: s, isAuthenticated: r } = ve();
  return r ? (Array.isArray(e) ? e : [e]).some((a) => s(a)) ? /* @__PURE__ */ p.jsx(p.Fragment, { children: t }) : /* @__PURE__ */ p.jsx(p.Fragment, { children: n }) : /* @__PURE__ */ p.jsx(p.Fragment, { children: n });
}
function ip({
  children: e,
  fallback: t = null
}) {
  const { isAdmin: n, isAuthenticated: s } = ve();
  return !s || !n ? /* @__PURE__ */ p.jsx(p.Fragment, { children: t }) : /* @__PURE__ */ p.jsx(p.Fragment, { children: e });
}
function Ll(e, t, n, s) {
  return zn({
    queryKey: e,
    queryFn: async () => await ne.get(t, { params: n }),
    ...s
  });
}
function Il(e, t, n, s) {
  return zn({
    queryKey: [...e, n],
    queryFn: async () => await ne.get(t, {
      params: n
    }),
    ...s
  });
}
function op(e, t = "post", n) {
  return Mt({
    mutationFn: async (s) => {
      switch (t) {
        case "post":
          return ne.post(e, s);
        case "put":
          return ne.put(e, s);
        case "patch":
          return ne.patch(e, s);
        case "delete":
          return ne.delete(e);
      }
    },
    ...n
  });
}
function ap(e, t) {
  const n = bi(), s = (c) => Il(
    [t, "list"],
    e,
    c
  ), r = (c, u) => Ll(
    [t, c],
    `${e}/${c}`,
    void 0,
    {
      enabled: !!c,
      ...u
    }
  ), o = (c) => Mt({
    mutationFn: (u) => ne.post(e, u),
    onSuccess: () => {
      n.invalidateQueries({ queryKey: [t] });
    },
    ...c
  }), i = (c) => Mt({
    mutationFn: ({ id: u, data: d }) => ne.put(`${e}/${u}`, d),
    onSuccess: (u, d) => {
      n.invalidateQueries({ queryKey: [t, d.id] }), n.invalidateQueries({ queryKey: [t, "list"] });
    },
    ...c
  }), a = (c) => Mt({
    mutationFn: (u) => ne.delete(`${e}/${u}`),
    onSuccess: () => {
      n.invalidateQueries({ queryKey: [t] });
    },
    ...c
  }), l = z(() => {
    n.invalidateQueries({ queryKey: [t] });
  }, [n]);
  return {
    useList: s,
    useOne: r,
    useCreate: o,
    useUpdate: i,
    useDelete: a,
    invalidate: l
  };
}
function lp() {
  const e = bi();
  return z(
    async (t, n, s) => {
      await e.prefetchQuery({
        queryKey: t,
        queryFn: () => ne.get(n, { params: s }),
        staleTime: 300 * 1e3
        // 5 minutes
      });
    },
    [e]
  );
}
function cp(e, t, n = 20) {
  return zn({
    queryKey: e,
    queryFn: async () => await ne.get(t, {
      params: { pageSize: n }
    })
  });
}
function up(e, t = "fa-IR") {
  return new Intl.NumberFormat(t).format(e);
}
function dp(e, t = "IRR", n = "fa-IR") {
  return new Intl.NumberFormat(n, {
    style: "currency",
    currency: t,
    minimumFractionDigits: 0
  }).format(e);
}
function hp(e) {
  if (e === 0) return "0 بایت";
  const t = ["بایت", "کیلوبایت", "مگابایت", "گیگابایت"], n = 1024, s = Math.floor(Math.log(e) / Math.log(n));
  return `${parseFloat((e / Math.pow(n, s)).toFixed(2))} ${t[s]}`;
}
function fp(e) {
  return new Promise((t) => setTimeout(t, e));
}
function pp(e, t) {
  let n = null;
  return function(...s) {
    n && clearTimeout(n), n = setTimeout(() => {
      e.apply(this, s), n = null;
    }, t);
  };
}
function mp(e, t) {
  let n = !1;
  return function(...s) {
    n || (e.apply(this, s), n = !0, setTimeout(() => {
      n = !1;
    }, t));
  };
}
function gp(e) {
  return JSON.parse(JSON.stringify(e));
}
function yp(e) {
  return e == null ? !0 : Array.isArray(e) ? e.length === 0 : typeof e == "object" ? Object.keys(e).length === 0 : typeof e == "string" ? e.trim().length === 0 : !1;
}
function vp(e = "") {
  const t = Date.now().toString(36), n = Math.random().toString(36).substring(2, 9);
  return e ? `${e}_${t}${n}` : `${t}${n}`;
}
function xp(e, t, n) {
  const s = t.split(".");
  let r = e;
  for (const o of s) {
    if (r == null)
      return n;
    r = r[o];
  }
  return r ?? n;
}
function Tp(e) {
  const t = [];
  for (const [n, s] of Object.entries(e))
    s != null && (Array.isArray(s) ? s.forEach((r) => {
      t.push(`${encodeURIComponent(n)}=${encodeURIComponent(String(r))}`);
    }) : t.push(`${encodeURIComponent(n)}=${encodeURIComponent(String(s))}`));
  return t.join("&");
}
function bp(e) {
  const t = {};
  return new URLSearchParams(e).forEach((s, r) => {
    t[r] ? Array.isArray(t[r]) ? t[r].push(s) : t[r] = [t[r], s] : t[r] = s;
  }), t;
}
function Ep(e, t, n = "...") {
  return e.length <= t ? e : e.substring(0, t - n.length) + n;
}
function Sp(e) {
  return e ? e.charAt(0).toUpperCase() + e.slice(1) : "";
}
function wp(e) {
  return e.toLowerCase().trim().replace(/[^\w\s-]/g, "").replace(/[\s_-]+/g, "-").replace(/^-+|-+$/g, "");
}
const Ap = {
  get(e, t) {
    try {
      const n = localStorage.getItem(e);
      return n ? JSON.parse(n) : t;
    } catch {
      return t;
    }
  },
  set(e, t) {
    try {
      localStorage.setItem(e, JSON.stringify(t));
    } catch (n) {
      console.error("Storage set error:", n);
    }
  },
  remove(e) {
    localStorage.removeItem(e);
  },
  clear() {
    localStorage.clear();
  }
};
function Rp(...e) {
  return e.filter(Boolean).join(" ");
}
const qn = Le({});
function Zn(e) {
  const t = de(null);
  return t.current === null && (t.current = e()), t.current;
}
const Zt = Le(null), Jn = Le({
  transformPagePoint: (e) => e,
  isStatic: !1,
  reducedMotion: "never"
});
class _l extends Hn.Component {
  getSnapshotBeforeUpdate(t) {
    const n = this.props.childRef.current;
    if (n && t.isPresent && !this.props.isPresent) {
      const s = this.props.sizeRef.current;
      s.height = n.offsetHeight || 0, s.width = n.offsetWidth || 0, s.top = n.offsetTop, s.left = n.offsetLeft;
    }
    return null;
  }
  /**
   * Required with getSnapshotBeforeUpdate to stop React complaining.
   */
  componentDidUpdate() {
  }
  render() {
    return this.props.children;
  }
}
function Fl({ children: e, isPresent: t }) {
  const n = Gn(), s = de(null), r = de({
    width: 0,
    height: 0,
    top: 0,
    left: 0
  }), { nonce: o } = X(Jn);
  return gi(() => {
    const { width: i, height: a, top: l, left: c } = r.current;
    if (t || !s.current || !i || !a)
      return;
    s.current.dataset.motionPopId = n;
    const u = document.createElement("style");
    return o && (u.nonce = o), document.head.appendChild(u), u.sheet && u.sheet.insertRule(`
          [data-motion-pop-id="${n}"] {
            position: absolute !important;
            width: ${i}px !important;
            height: ${a}px !important;
            top: ${l}px !important;
            left: ${c}px !important;
          }
        `), () => {
      document.head.removeChild(u);
    };
  }, [t]), p.jsx(_l, { isPresent: t, childRef: s, sizeRef: r, children: Hn.cloneElement(e, { ref: s }) });
}
const Bl = ({ children: e, initial: t, isPresent: n, onExitComplete: s, custom: r, presenceAffectsLayout: o, mode: i }) => {
  const a = Zn(Ul), l = Gn(), c = z((d) => {
    a.set(d, !0);
    for (const f of a.values())
      if (!f)
        return;
    s && s();
  }, [a, s]), u = he(
    () => ({
      id: l,
      initial: t,
      isPresent: n,
      custom: r,
      onExitComplete: c,
      register: (d) => (a.set(d, !1), () => a.delete(d))
    }),
    /**
     * If the presence of a child affects the layout of the components around it,
     * we want to make a new context value to ensure they get re-rendered
     * so they can detect that layout change.
     */
    o ? [Math.random(), c] : [n, c]
  );
  return he(() => {
    a.forEach((d, f) => a.set(f, !1));
  }, [n]), Hn.useEffect(() => {
    !n && !a.size && s && s();
  }, [n]), i === "popLayout" && (e = p.jsx(Fl, { isPresent: n, children: e })), p.jsx(Zt.Provider, { value: u, children: e });
};
function Ul() {
  return /* @__PURE__ */ new Map();
}
function Oi(e = !0) {
  const t = X(Zt);
  if (t === null)
    return [!0, null];
  const { isPresent: n, onExitComplete: s, register: r } = t, o = Gn();
  Ve(() => {
    e && r(o);
  }, [e]);
  const i = z(() => e && s && s(o), [o, s, e]);
  return !n && s ? [!1, i] : [!0];
}
const jt = (e) => e.key || "";
function Qs(e) {
  const t = [];
  return Ua.forEach(e, (n) => {
    $a(n) && t.push(n);
  }), t;
}
const Qn = typeof window < "u", Li = Qn ? Ka : Ve, Ye = ({ children: e, custom: t, initial: n = !0, onExitComplete: s, presenceAffectsLayout: r = !0, mode: o = "sync", propagate: i = !1 }) => {
  const [a, l] = Oi(i), c = he(() => Qs(e), [e]), u = i && !a ? [] : c.map(jt), d = de(!0), f = de(c), m = Zn(() => /* @__PURE__ */ new Map()), [g, v] = Ft(c), [T, x] = Ft(c);
  Li(() => {
    d.current = !1, f.current = c;
    for (let P = 0; P < T.length; P++) {
      const S = jt(T[P]);
      u.includes(S) ? m.delete(S) : m.get(S) !== !0 && m.set(S, !1);
    }
  }, [T, u.length, u.join("-")]);
  const b = [];
  if (c !== g) {
    let P = [...c];
    for (let S = 0; S < T.length; S++) {
      const j = T[S], O = jt(j);
      u.includes(O) || (P.splice(S, 0, j), b.push(j));
    }
    o === "wait" && b.length && (P = b), x(Qs(P)), v(c);
    return;
  }
  process.env.NODE_ENV !== "production" && o === "wait" && T.length > 1 && console.warn(`You're attempting to animate multiple children within AnimatePresence, but its mode is set to "wait". This will lead to odd visual behaviour.`);
  const { forceRender: w } = X(qn);
  return p.jsx(p.Fragment, { children: T.map((P) => {
    const S = jt(P), j = i && !a ? !1 : c === T || u.includes(S), O = () => {
      if (m.has(S))
        m.set(S, !0);
      else
        return;
      let C = !0;
      m.forEach((D) => {
        D || (C = !1);
      }), C && (w == null || w(), x(f.current), i && (l == null || l()), s && s());
    };
    return p.jsx(Bl, { isPresent: j, initial: !d.current || n ? void 0 : !1, custom: j ? void 0 : t, presenceAffectsLayout: r, mode: o, onExitComplete: j ? void 0 : O, children: P }, S);
  }) });
}, ee = /* @__NO_SIDE_EFFECTS__ */ (e) => e;
let Je = ee, Ae = ee;
process.env.NODE_ENV !== "production" && (Je = (e, t) => {
  !e && typeof console < "u" && console.warn(t);
}, Ae = (e, t) => {
  if (!e)
    throw new Error(t);
});
// @__NO_SIDE_EFFECTS__
function es(e) {
  let t;
  return () => (t === void 0 && (t = e()), t);
}
const Xe = /* @__NO_SIDE_EFFECTS__ */ (e, t, n) => {
  const s = t - e;
  return s === 0 ? 1 : (n - e) / s;
}, me = /* @__NO_SIDE_EFFECTS__ */ (e) => e * 1e3, xe = /* @__NO_SIDE_EFFECTS__ */ (e) => e / 1e3, $l = {
  useManualTiming: !1
};
function Kl(e) {
  let t = /* @__PURE__ */ new Set(), n = /* @__PURE__ */ new Set(), s = !1, r = !1;
  const o = /* @__PURE__ */ new WeakSet();
  let i = {
    delta: 0,
    timestamp: 0,
    isProcessing: !1
  };
  function a(c) {
    o.has(c) && (l.schedule(c), e()), c(i);
  }
  const l = {
    /**
     * Schedule a process to run on the next frame.
     */
    schedule: (c, u = !1, d = !1) => {
      const m = d && s ? t : n;
      return u && o.add(c), m.has(c) || m.add(c), c;
    },
    /**
     * Cancel the provided callback from running on the next frame.
     */
    cancel: (c) => {
      n.delete(c), o.delete(c);
    },
    /**
     * Execute all schedule callbacks.
     */
    process: (c) => {
      if (i = c, s) {
        r = !0;
        return;
      }
      s = !0, [t, n] = [n, t], t.forEach(a), t.clear(), s = !1, r && (r = !1, l.process(c));
    }
  };
  return l;
}
const kt = [
  "read",
  // Read
  "resolveKeyframes",
  // Write/Read/Write/Read
  "update",
  // Compute
  "preRender",
  // Compute
  "render",
  // Write
  "postRender"
  // Compute
], Wl = 40;
function Ii(e, t) {
  let n = !1, s = !0;
  const r = {
    delta: 0,
    timestamp: 0,
    isProcessing: !1
  }, o = () => n = !0, i = kt.reduce((x, b) => (x[b] = Kl(o), x), {}), { read: a, resolveKeyframes: l, update: c, preRender: u, render: d, postRender: f } = i, m = () => {
    const x = performance.now();
    n = !1, r.delta = s ? 1e3 / 60 : Math.max(Math.min(x - r.timestamp, Wl), 1), r.timestamp = x, r.isProcessing = !0, a.process(r), l.process(r), c.process(r), u.process(r), d.process(r), f.process(r), r.isProcessing = !1, n && t && (s = !1, e(m));
  }, g = () => {
    n = !0, s = !0, r.isProcessing || e(m);
  };
  return { schedule: kt.reduce((x, b) => {
    const w = i[b];
    return x[b] = (P, S = !1, j = !1) => (n || g(), w.schedule(P, S, j)), x;
  }, {}), cancel: (x) => {
    for (let b = 0; b < kt.length; b++)
      i[kt[b]].cancel(x);
  }, state: r, steps: i };
}
const { schedule: _, cancel: Re, state: G, steps: gn } = Ii(typeof requestAnimationFrame < "u" ? requestAnimationFrame : ee, !0), _i = Le({ strict: !1 }), er = {
  animation: [
    "animate",
    "variants",
    "whileHover",
    "whileTap",
    "exit",
    "whileInView",
    "whileFocus",
    "whileDrag"
  ],
  exit: ["exit"],
  drag: ["drag", "dragControls"],
  focus: ["whileFocus"],
  hover: ["whileHover", "onHoverStart", "onHoverEnd"],
  tap: ["whileTap", "onTap", "onTapStart", "onTapCancel"],
  pan: ["onPan", "onPanStart", "onPanSessionStart", "onPanEnd"],
  inView: ["whileInView", "onViewportEnter", "onViewportLeave"],
  layout: ["layout", "layoutId"]
}, qe = {};
for (const e in er)
  qe[e] = {
    isEnabled: (t) => er[e].some((n) => !!t[n])
  };
function Hl(e) {
  for (const t in e)
    qe[t] = {
      ...qe[t],
      ...e[t]
    };
}
const Gl = /* @__PURE__ */ new Set([
  "animate",
  "exit",
  "variants",
  "initial",
  "style",
  "values",
  "variants",
  "transition",
  "transformTemplate",
  "custom",
  "inherit",
  "onBeforeLayoutMeasure",
  "onAnimationStart",
  "onAnimationComplete",
  "onUpdate",
  "onDragStart",
  "onDrag",
  "onDragEnd",
  "onMeasureDragConstraints",
  "onDirectionLock",
  "onDragTransitionEnd",
  "_dragX",
  "_dragY",
  "onHoverStart",
  "onHoverEnd",
  "onViewportEnter",
  "onViewportLeave",
  "globalTapTarget",
  "ignoreStrict",
  "viewport"
]);
function Ut(e) {
  return e.startsWith("while") || e.startsWith("drag") && e !== "draggable" || e.startsWith("layout") || e.startsWith("onTap") || e.startsWith("onPan") || e.startsWith("onLayout") || Gl.has(e);
}
let Fi = (e) => !Ut(e);
function zl(e) {
  e && (Fi = (t) => t.startsWith("on") ? !Ut(t) : e(t));
}
try {
  zl(require("@emotion/is-prop-valid").default);
} catch {
}
function Yl(e, t, n) {
  const s = {};
  for (const r in e)
    r === "values" && typeof e.values == "object" || (Fi(r) || n === !0 && Ut(r) || !t && !Ut(r) || // If trying to use native HTML drag events, forward drag listeners
    e.draggable && r.startsWith("onDrag")) && (s[r] = e[r]);
  return s;
}
const tr = /* @__PURE__ */ new Set();
function Jt(e, t, n) {
  e || tr.has(t) || (console.warn(t), tr.add(t));
}
function Xl(e) {
  if (typeof Proxy > "u")
    return e;
  const t = /* @__PURE__ */ new Map(), n = (...s) => (process.env.NODE_ENV !== "production" && Jt(!1, "motion() is deprecated. Use motion.create() instead."), e(...s));
  return new Proxy(n, {
    /**
     * Called when `motion` is referenced with a prop: `motion.div`, `motion.input` etc.
     * The prop name is passed through as `key` and we can use that to generate a `motion`
     * DOM component with that name.
     */
    get: (s, r) => r === "create" ? e : (t.has(r) || t.set(r, e(r)), t.get(r))
  });
}
const Qt = Le({});
function gt(e) {
  return typeof e == "string" || Array.isArray(e);
}
function en(e) {
  return e !== null && typeof e == "object" && typeof e.start == "function";
}
const ts = [
  "animate",
  "whileInView",
  "whileFocus",
  "whileHover",
  "whileTap",
  "whileDrag",
  "exit"
], ns = ["initial", ...ts];
function tn(e) {
  return en(e.animate) || ns.some((t) => gt(e[t]));
}
function Bi(e) {
  return !!(tn(e) || e.variants);
}
function ql(e, t) {
  if (tn(e)) {
    const { initial: n, animate: s } = e;
    return {
      initial: n === !1 || gt(n) ? n : void 0,
      animate: gt(s) ? s : void 0
    };
  }
  return e.inherit !== !1 ? t : {};
}
function Zl(e) {
  const { initial: t, animate: n } = ql(e, X(Qt));
  return he(() => ({ initial: t, animate: n }), [nr(t), nr(n)]);
}
function nr(e) {
  return Array.isArray(e) ? e.join(" ") : e;
}
const Jl = Symbol.for("motionComponentSymbol");
function Ke(e) {
  return e && typeof e == "object" && Object.prototype.hasOwnProperty.call(e, "current");
}
function Ql(e, t, n) {
  return z(
    (s) => {
      s && e.onMount && e.onMount(s), t && (s ? t.mount(s) : t.unmount()), n && (typeof n == "function" ? n(s) : Ke(n) && (n.current = s));
    },
    /**
     * Only pass a new ref callback to React if we've received a visual element
     * factory. Otherwise we'll be mounting/remounting every time externalRef
     * or other dependencies change.
     */
    [t]
  );
}
const ss = (e) => e.replace(/([a-z])([A-Z])/gu, "$1-$2").toLowerCase(), ec = "framerAppearId", Ui = "data-" + ss(ec), { schedule: rs } = Ii(queueMicrotask, !1), $i = Le({});
function tc(e, t, n, s, r) {
  var o, i;
  const { visualElement: a } = X(Qt), l = X(_i), c = X(Zt), u = X(Jn).reducedMotion, d = de(null);
  s = s || l.renderer, !d.current && s && (d.current = s(e, {
    visualState: t,
    parent: a,
    props: n,
    presenceContext: c,
    blockInitialAnimation: c ? c.initial === !1 : !1,
    reducedMotionConfig: u
  }));
  const f = d.current, m = X($i);
  f && !f.projection && r && (f.type === "html" || f.type === "svg") && nc(d.current, n, r, m);
  const g = de(!1);
  gi(() => {
    f && g.current && f.update(n, c);
  });
  const v = n[Ui], T = de(!!v && !(!((o = window.MotionHandoffIsComplete) === null || o === void 0) && o.call(window, v)) && ((i = window.MotionHasOptimisedAnimation) === null || i === void 0 ? void 0 : i.call(window, v)));
  return Li(() => {
    f && (g.current = !0, window.MotionIsMounted = !0, f.updateFeatures(), rs.render(f.render), T.current && f.animationState && f.animationState.animateChanges());
  }), Ve(() => {
    f && (!T.current && f.animationState && f.animationState.animateChanges(), T.current && (queueMicrotask(() => {
      var x;
      (x = window.MotionHandoffMarkAsComplete) === null || x === void 0 || x.call(window, v);
    }), T.current = !1));
  }), f;
}
function nc(e, t, n, s) {
  const { layoutId: r, layout: o, drag: i, dragConstraints: a, layoutScroll: l, layoutRoot: c } = t;
  e.projection = new n(e.latestValues, t["data-framer-portal-id"] ? void 0 : Ki(e.parent)), e.projection.setOptions({
    layoutId: r,
    layout: o,
    alwaysMeasureLayout: !!i || a && Ke(a),
    visualElement: e,
    /**
     * TODO: Update options in an effect. This could be tricky as it'll be too late
     * to update by the time layout animations run.
     * We also need to fix this safeToRemove by linking it up to the one returned by usePresence,
     * ensuring it gets called if there's no potential layout animations.
     *
     */
    animationType: typeof o == "string" ? o : "both",
    initialPromotionConfig: s,
    layoutScroll: l,
    layoutRoot: c
  });
}
function Ki(e) {
  if (e)
    return e.options.allowProjection !== !1 ? e.projection : Ki(e.parent);
}
function sc({ preloadedFeatures: e, createVisualElement: t, useRender: n, useVisualState: s, Component: r }) {
  var o, i;
  e && Hl(e);
  function a(c, u) {
    let d;
    const f = {
      ...X(Jn),
      ...c,
      layoutId: rc(c)
    }, { isStatic: m } = f, g = Zl(c), v = s(c, m);
    if (!m && Qn) {
      ic(f, e);
      const T = oc(f);
      d = T.MeasureLayout, g.visualElement = tc(r, v, f, t, T.ProjectionNode);
    }
    return p.jsxs(Qt.Provider, { value: g, children: [d && g.visualElement ? p.jsx(d, { visualElement: g.visualElement, ...f }) : null, n(r, c, Ql(v, g.visualElement, u), v, m, g.visualElement)] });
  }
  a.displayName = `motion.${typeof r == "string" ? r : `create(${(i = (o = r.displayName) !== null && o !== void 0 ? o : r.name) !== null && i !== void 0 ? i : ""})`}`;
  const l = Wa(a);
  return l[Jl] = r, l;
}
function rc({ layoutId: e }) {
  const t = X(qn).id;
  return t && e !== void 0 ? t + "-" + e : e;
}
function ic(e, t) {
  const n = X(_i).strict;
  if (process.env.NODE_ENV !== "production" && t && n) {
    const s = "You have rendered a `motion` component within a `LazyMotion` component. This will break tree shaking. Import and render a `m` component instead.";
    e.ignoreStrict ? Je(!1, s) : Ae(!1, s);
  }
}
function oc(e) {
  const { drag: t, layout: n } = qe;
  if (!t && !n)
    return {};
  const s = { ...t, ...n };
  return {
    MeasureLayout: t != null && t.isEnabled(e) || n != null && n.isEnabled(e) ? s.MeasureLayout : void 0,
    ProjectionNode: s.ProjectionNode
  };
}
const ac = [
  "animate",
  "circle",
  "defs",
  "desc",
  "ellipse",
  "g",
  "image",
  "line",
  "filter",
  "marker",
  "mask",
  "metadata",
  "path",
  "pattern",
  "polygon",
  "polyline",
  "rect",
  "stop",
  "switch",
  "symbol",
  "svg",
  "text",
  "tspan",
  "use",
  "view"
];
function is(e) {
  return (
    /**
     * If it's not a string, it's a custom React component. Currently we only support
     * HTML custom React components.
     */
    typeof e != "string" || /**
     * If it contains a dash, the element is a custom HTML webcomponent.
     */
    e.includes("-") ? !1 : (
      /**
       * If it's in our list of lowercase SVG tags, it's an SVG component
       */
      !!(ac.indexOf(e) > -1 || /**
       * If it contains a capital letter, it's an SVG component
       */
      /[A-Z]/u.test(e))
    )
  );
}
function sr(e) {
  const t = [{}, {}];
  return e == null || e.values.forEach((n, s) => {
    t[0][s] = n.get(), t[1][s] = n.getVelocity();
  }), t;
}
function os(e, t, n, s) {
  if (typeof t == "function") {
    const [r, o] = sr(s);
    t = t(n !== void 0 ? n : e.custom, r, o);
  }
  if (typeof t == "string" && (t = e.variants && e.variants[t]), typeof t == "function") {
    const [r, o] = sr(s);
    t = t(n !== void 0 ? n : e.custom, r, o);
  }
  return t;
}
const Dn = (e) => Array.isArray(e), lc = (e) => !!(e && typeof e == "object" && e.mix && e.toValue), cc = (e) => Dn(e) ? e[e.length - 1] || 0 : e, Z = (e) => !!(e && e.getVelocity);
function Lt(e) {
  const t = Z(e) ? e.get() : e;
  return lc(t) ? t.toValue() : t;
}
function uc({ scrapeMotionValuesFromProps: e, createRenderState: t, onUpdate: n }, s, r, o) {
  const i = {
    latestValues: dc(s, r, o, e),
    renderState: t()
  };
  return n && (i.onMount = (a) => n({ props: s, current: a, ...i }), i.onUpdate = (a) => n(a)), i;
}
const Wi = (e) => (t, n) => {
  const s = X(Qt), r = X(Zt), o = () => uc(e, t, s, r);
  return n ? o() : Zn(o);
};
function dc(e, t, n, s) {
  const r = {}, o = s(e, {});
  for (const f in o)
    r[f] = Lt(o[f]);
  let { initial: i, animate: a } = e;
  const l = tn(e), c = Bi(e);
  t && c && !l && e.inherit !== !1 && (i === void 0 && (i = t.initial), a === void 0 && (a = t.animate));
  let u = n ? n.initial === !1 : !1;
  u = u || i === !1;
  const d = u ? a : i;
  if (d && typeof d != "boolean" && !en(d)) {
    const f = Array.isArray(d) ? d : [d];
    for (let m = 0; m < f.length; m++) {
      const g = os(e, f[m]);
      if (g) {
        const { transitionEnd: v, transition: T, ...x } = g;
        for (const b in x) {
          let w = x[b];
          if (Array.isArray(w)) {
            const P = u ? w.length - 1 : 0;
            w = w[P];
          }
          w !== null && (r[b] = w);
        }
        for (const b in v)
          r[b] = v[b];
      }
    }
  }
  return r;
}
const Qe = [
  "transformPerspective",
  "x",
  "y",
  "z",
  "translateX",
  "translateY",
  "translateZ",
  "scale",
  "scaleX",
  "scaleY",
  "rotate",
  "rotateX",
  "rotateY",
  "rotateZ",
  "skew",
  "skewX",
  "skewY"
], Ie = new Set(Qe), Hi = (e) => (t) => typeof t == "string" && t.startsWith(e), Gi = /* @__PURE__ */ Hi("--"), hc = /* @__PURE__ */ Hi("var(--"), as = (e) => hc(e) ? fc.test(e.split("/*")[0].trim()) : !1, fc = /var\(--(?:[\w-]+\s*|[\w-]+\s*,(?:\s*[^)(\s]|\s*\((?:[^)(]|\([^)(]*\))*\))+\s*)\)$/iu, zi = (e, t) => t && typeof e == "number" ? t.transform(e) : e, be = (e, t, n) => n > t ? t : n < e ? e : n, et = {
  test: (e) => typeof e == "number",
  parse: parseFloat,
  transform: (e) => e
}, yt = {
  ...et,
  transform: (e) => be(0, 1, e)
}, Nt = {
  ...et,
  default: 1
}, bt = (e) => ({
  test: (t) => typeof t == "string" && t.endsWith(e) && t.split(" ").length === 1,
  parse: parseFloat,
  transform: (t) => `${t}${e}`
}), we = /* @__PURE__ */ bt("deg"), ge = /* @__PURE__ */ bt("%"), A = /* @__PURE__ */ bt("px"), pc = /* @__PURE__ */ bt("vh"), mc = /* @__PURE__ */ bt("vw"), rr = {
  ...ge,
  parse: (e) => ge.parse(e) / 100,
  transform: (e) => ge.transform(e * 100)
}, gc = {
  // Border props
  borderWidth: A,
  borderTopWidth: A,
  borderRightWidth: A,
  borderBottomWidth: A,
  borderLeftWidth: A,
  borderRadius: A,
  radius: A,
  borderTopLeftRadius: A,
  borderTopRightRadius: A,
  borderBottomRightRadius: A,
  borderBottomLeftRadius: A,
  // Positioning props
  width: A,
  maxWidth: A,
  height: A,
  maxHeight: A,
  top: A,
  right: A,
  bottom: A,
  left: A,
  // Spacing props
  padding: A,
  paddingTop: A,
  paddingRight: A,
  paddingBottom: A,
  paddingLeft: A,
  margin: A,
  marginTop: A,
  marginRight: A,
  marginBottom: A,
  marginLeft: A,
  // Misc
  backgroundPositionX: A,
  backgroundPositionY: A
}, yc = {
  rotate: we,
  rotateX: we,
  rotateY: we,
  rotateZ: we,
  scale: Nt,
  scaleX: Nt,
  scaleY: Nt,
  scaleZ: Nt,
  skew: we,
  skewX: we,
  skewY: we,
  distance: A,
  translateX: A,
  translateY: A,
  translateZ: A,
  x: A,
  y: A,
  z: A,
  perspective: A,
  transformPerspective: A,
  opacity: yt,
  originX: rr,
  originY: rr,
  originZ: A
}, ir = {
  ...et,
  transform: Math.round
}, ls = {
  ...gc,
  ...yc,
  zIndex: ir,
  size: A,
  // SVG
  fillOpacity: yt,
  strokeOpacity: yt,
  numOctaves: ir
}, vc = {
  x: "translateX",
  y: "translateY",
  z: "translateZ",
  transformPerspective: "perspective"
}, xc = Qe.length;
function Tc(e, t, n) {
  let s = "", r = !0;
  for (let o = 0; o < xc; o++) {
    const i = Qe[o], a = e[i];
    if (a === void 0)
      continue;
    let l = !0;
    if (typeof a == "number" ? l = a === (i.startsWith("scale") ? 1 : 0) : l = parseFloat(a) === 0, !l || n) {
      const c = zi(a, ls[i]);
      if (!l) {
        r = !1;
        const u = vc[i] || i;
        s += `${u}(${c}) `;
      }
      n && (t[i] = c);
    }
  }
  return s = s.trim(), n ? s = n(t, r ? "" : s) : r && (s = "none"), s;
}
function cs(e, t, n) {
  const { style: s, vars: r, transformOrigin: o } = e;
  let i = !1, a = !1;
  for (const l in t) {
    const c = t[l];
    if (Ie.has(l)) {
      i = !0;
      continue;
    } else if (Gi(l)) {
      r[l] = c;
      continue;
    } else {
      const u = zi(c, ls[l]);
      l.startsWith("origin") ? (a = !0, o[l] = u) : s[l] = u;
    }
  }
  if (t.transform || (i || n ? s.transform = Tc(t, e.transform, n) : s.transform && (s.transform = "none")), a) {
    const { originX: l = "50%", originY: c = "50%", originZ: u = 0 } = o;
    s.transformOrigin = `${l} ${c} ${u}`;
  }
}
const bc = {
  offset: "stroke-dashoffset",
  array: "stroke-dasharray"
}, Ec = {
  offset: "strokeDashoffset",
  array: "strokeDasharray"
};
function Sc(e, t, n = 1, s = 0, r = !0) {
  e.pathLength = 1;
  const o = r ? bc : Ec;
  e[o.offset] = A.transform(-s);
  const i = A.transform(t), a = A.transform(n);
  e[o.array] = `${i} ${a}`;
}
function or(e, t, n) {
  return typeof e == "string" ? e : A.transform(t + n * e);
}
function wc(e, t, n) {
  const s = or(t, e.x, e.width), r = or(n, e.y, e.height);
  return `${s} ${r}`;
}
function us(e, {
  attrX: t,
  attrY: n,
  attrScale: s,
  originX: r,
  originY: o,
  pathLength: i,
  pathSpacing: a = 1,
  pathOffset: l = 0,
  // This is object creation, which we try to avoid per-frame.
  ...c
}, u, d) {
  if (cs(e, c, d), u) {
    e.style.viewBox && (e.attrs.viewBox = e.style.viewBox);
    return;
  }
  e.attrs = e.style, e.style = {};
  const { attrs: f, style: m, dimensions: g } = e;
  f.transform && (g && (m.transform = f.transform), delete f.transform), g && (r !== void 0 || o !== void 0 || m.transform) && (m.transformOrigin = wc(g, r !== void 0 ? r : 0.5, o !== void 0 ? o : 0.5)), t !== void 0 && (f.x = t), n !== void 0 && (f.y = n), s !== void 0 && (f.scale = s), i !== void 0 && Sc(f, i, a, l, !1);
}
const ds = () => ({
  style: {},
  transform: {},
  transformOrigin: {},
  vars: {}
}), Yi = () => ({
  ...ds(),
  attrs: {}
}), hs = (e) => typeof e == "string" && e.toLowerCase() === "svg";
function Xi(e, { style: t, vars: n }, s, r) {
  Object.assign(e.style, t, r && r.getProjectionStyles(s));
  for (const o in n)
    e.style.setProperty(o, n[o]);
}
const qi = /* @__PURE__ */ new Set([
  "baseFrequency",
  "diffuseConstant",
  "kernelMatrix",
  "kernelUnitLength",
  "keySplines",
  "keyTimes",
  "limitingConeAngle",
  "markerHeight",
  "markerWidth",
  "numOctaves",
  "targetX",
  "targetY",
  "surfaceScale",
  "specularConstant",
  "specularExponent",
  "stdDeviation",
  "tableValues",
  "viewBox",
  "gradientTransform",
  "pathLength",
  "startOffset",
  "textLength",
  "lengthAdjust"
]);
function Zi(e, t, n, s) {
  Xi(e, t, void 0, s);
  for (const r in t.attrs)
    e.setAttribute(qi.has(r) ? r : ss(r), t.attrs[r]);
}
const $t = {};
function Ac(e) {
  Object.assign($t, e);
}
function Ji(e, { layout: t, layoutId: n }) {
  return Ie.has(e) || e.startsWith("origin") || (t || n !== void 0) && (!!$t[e] || e === "opacity");
}
function fs(e, t, n) {
  var s;
  const { style: r } = e, o = {};
  for (const i in r)
    (Z(r[i]) || t.style && Z(t.style[i]) || Ji(i, e) || ((s = n == null ? void 0 : n.getValue(i)) === null || s === void 0 ? void 0 : s.liveStyle) !== void 0) && (o[i] = r[i]);
  return o;
}
function Qi(e, t, n) {
  const s = fs(e, t, n);
  for (const r in e)
    if (Z(e[r]) || Z(t[r])) {
      const o = Qe.indexOf(r) !== -1 ? "attr" + r.charAt(0).toUpperCase() + r.substring(1) : r;
      s[o] = e[r];
    }
  return s;
}
function Rc(e, t) {
  try {
    t.dimensions = typeof e.getBBox == "function" ? e.getBBox() : e.getBoundingClientRect();
  } catch {
    t.dimensions = {
      x: 0,
      y: 0,
      width: 0,
      height: 0
    };
  }
}
const ar = ["x", "y", "width", "height", "cx", "cy", "r"], Pc = {
  useVisualState: Wi({
    scrapeMotionValuesFromProps: Qi,
    createRenderState: Yi,
    onUpdate: ({ props: e, prevProps: t, current: n, renderState: s, latestValues: r }) => {
      if (!n)
        return;
      let o = !!e.drag;
      if (!o) {
        for (const a in r)
          if (Ie.has(a)) {
            o = !0;
            break;
          }
      }
      if (!o)
        return;
      let i = !t;
      if (t)
        for (let a = 0; a < ar.length; a++) {
          const l = ar[a];
          e[l] !== t[l] && (i = !0);
        }
      i && _.read(() => {
        Rc(n, s), _.render(() => {
          us(s, r, hs(n.tagName), e.transformTemplate), Zi(n, s);
        });
      });
    }
  })
}, Cc = {
  useVisualState: Wi({
    scrapeMotionValuesFromProps: fs,
    createRenderState: ds
  })
};
function eo(e, t, n) {
  for (const s in t)
    !Z(t[s]) && !Ji(s, n) && (e[s] = t[s]);
}
function Dc({ transformTemplate: e }, t) {
  return he(() => {
    const n = ds();
    return cs(n, t, e), Object.assign({}, n.vars, n.style);
  }, [t]);
}
function jc(e, t) {
  const n = e.style || {}, s = {};
  return eo(s, n, e), Object.assign(s, Dc(e, t)), s;
}
function kc(e, t) {
  const n = {}, s = jc(e, t);
  return e.drag && e.dragListener !== !1 && (n.draggable = !1, s.userSelect = s.WebkitUserSelect = s.WebkitTouchCallout = "none", s.touchAction = e.drag === !0 ? "none" : `pan-${e.drag === "x" ? "y" : "x"}`), e.tabIndex === void 0 && (e.onTap || e.onTapStart || e.whileTap) && (n.tabIndex = 0), n.style = s, n;
}
function Nc(e, t, n, s) {
  const r = he(() => {
    const o = Yi();
    return us(o, t, hs(s), e.transformTemplate), {
      ...o.attrs,
      style: { ...o.style }
    };
  }, [t]);
  if (e.style) {
    const o = {};
    eo(o, e.style, e), r.style = { ...o, ...r.style };
  }
  return r;
}
function Vc(e = !1) {
  return (n, s, r, { latestValues: o }, i) => {
    const l = (is(n) ? Nc : kc)(s, o, i, n), c = Yl(s, typeof n == "string", e), u = n !== yi ? { ...c, ...l, ref: r } : {}, { children: d } = s, f = he(() => Z(d) ? d.get() : d, [d]);
    return Ha(n, {
      ...u,
      children: f
    });
  };
}
function Mc(e, t) {
  return function(s, { forwardMotionProps: r } = { forwardMotionProps: !1 }) {
    const i = {
      ...is(s) ? Pc : Cc,
      preloadedFeatures: e,
      useRender: Vc(r),
      createVisualElement: t,
      Component: s
    };
    return sc(i);
  };
}
function to(e, t) {
  if (!Array.isArray(t))
    return !1;
  const n = t.length;
  if (n !== e.length)
    return !1;
  for (let s = 0; s < n; s++)
    if (t[s] !== e[s])
      return !1;
  return !0;
}
function nn(e, t, n) {
  const s = e.getProps();
  return os(s, t, n !== void 0 ? n : s.custom, e);
}
const Oc = /* @__PURE__ */ es(() => window.ScrollTimeline !== void 0);
class Lc {
  constructor(t) {
    this.stop = () => this.runAll("stop"), this.animations = t.filter(Boolean);
  }
  get finished() {
    return Promise.all(this.animations.map((t) => "finished" in t ? t.finished : t));
  }
  /**
   * TODO: Filter out cancelled or stopped animations before returning
   */
  getAll(t) {
    return this.animations[0][t];
  }
  setAll(t, n) {
    for (let s = 0; s < this.animations.length; s++)
      this.animations[s][t] = n;
  }
  attachTimeline(t, n) {
    const s = this.animations.map((r) => {
      if (Oc() && r.attachTimeline)
        return r.attachTimeline(t);
      if (typeof n == "function")
        return n(r);
    });
    return () => {
      s.forEach((r, o) => {
        r && r(), this.animations[o].stop();
      });
    };
  }
  get time() {
    return this.getAll("time");
  }
  set time(t) {
    this.setAll("time", t);
  }
  get speed() {
    return this.getAll("speed");
  }
  set speed(t) {
    this.setAll("speed", t);
  }
  get startTime() {
    return this.getAll("startTime");
  }
  get duration() {
    let t = 0;
    for (let n = 0; n < this.animations.length; n++)
      t = Math.max(t, this.animations[n].duration);
    return t;
  }
  runAll(t) {
    this.animations.forEach((n) => n[t]());
  }
  flatten() {
    this.runAll("flatten");
  }
  play() {
    this.runAll("play");
  }
  pause() {
    this.runAll("pause");
  }
  cancel() {
    this.runAll("cancel");
  }
  complete() {
    this.runAll("complete");
  }
}
class Ic extends Lc {
  then(t, n) {
    return Promise.all(this.animations).then(t).catch(n);
  }
}
function ps(e, t) {
  return e ? e[t] || e.default || e : void 0;
}
const jn = 2e4;
function no(e) {
  let t = 0;
  const n = 50;
  let s = e.next(t);
  for (; !s.done && t < jn; )
    t += n, s = e.next(t);
  return t >= jn ? 1 / 0 : t;
}
function ms(e) {
  return typeof e == "function";
}
function lr(e, t) {
  e.timeline = t, e.onfinish = null;
}
const gs = (e) => Array.isArray(e) && typeof e[0] == "number", _c = {
  linearEasing: void 0
};
function Fc(e, t) {
  const n = /* @__PURE__ */ es(e);
  return () => {
    var s;
    return (s = _c[t]) !== null && s !== void 0 ? s : n();
  };
}
const Kt = /* @__PURE__ */ Fc(() => {
  try {
    document.createElement("div").animate({ opacity: 0 }, { easing: "linear(0, 1)" });
  } catch {
    return !1;
  }
  return !0;
}, "linearEasing"), so = (e, t, n = 10) => {
  let s = "";
  const r = Math.max(Math.round(t / n), 2);
  for (let o = 0; o < r; o++)
    s += e(/* @__PURE__ */ Xe(0, r - 1, o)) + ", ";
  return `linear(${s.substring(0, s.length - 2)})`;
};
function ro(e) {
  return !!(typeof e == "function" && Kt() || !e || typeof e == "string" && (e in kn || Kt()) || gs(e) || Array.isArray(e) && e.every(ro));
}
const at = ([e, t, n, s]) => `cubic-bezier(${e}, ${t}, ${n}, ${s})`, kn = {
  linear: "linear",
  ease: "ease",
  easeIn: "ease-in",
  easeOut: "ease-out",
  easeInOut: "ease-in-out",
  circIn: /* @__PURE__ */ at([0, 0.65, 0.55, 1]),
  circOut: /* @__PURE__ */ at([0.55, 0, 1, 0.45]),
  backIn: /* @__PURE__ */ at([0.31, 0.01, 0.66, -0.59]),
  backOut: /* @__PURE__ */ at([0.33, 1.53, 0.69, 0.99])
};
function io(e, t) {
  if (e)
    return typeof e == "function" && Kt() ? so(e, t) : gs(e) ? at(e) : Array.isArray(e) ? e.map((n) => io(n, t) || kn.easeOut) : kn[e];
}
const ue = {
  x: !1,
  y: !1
};
function oo() {
  return ue.x || ue.y;
}
function Bc(e, t, n) {
  var s;
  if (e instanceof Element)
    return [e];
  if (typeof e == "string") {
    let r = document;
    const o = (s = void 0) !== null && s !== void 0 ? s : r.querySelectorAll(e);
    return o ? Array.from(o) : [];
  }
  return Array.from(e);
}
function ao(e, t) {
  const n = Bc(e), s = new AbortController(), r = {
    passive: !0,
    ...t,
    signal: s.signal
  };
  return [n, r, () => s.abort()];
}
function cr(e) {
  return (t) => {
    t.pointerType === "touch" || oo() || e(t);
  };
}
function Uc(e, t, n = {}) {
  const [s, r, o] = ao(e, n), i = cr((a) => {
    const { target: l } = a, c = t(a);
    if (typeof c != "function" || !l)
      return;
    const u = cr((d) => {
      c(d), l.removeEventListener("pointerleave", u);
    });
    l.addEventListener("pointerleave", u, r);
  });
  return s.forEach((a) => {
    a.addEventListener("pointerenter", i, r);
  }), o;
}
const lo = (e, t) => t ? e === t ? !0 : lo(e, t.parentElement) : !1, ys = (e) => e.pointerType === "mouse" ? typeof e.button != "number" || e.button <= 0 : e.isPrimary !== !1, $c = /* @__PURE__ */ new Set([
  "BUTTON",
  "INPUT",
  "SELECT",
  "TEXTAREA",
  "A"
]);
function Kc(e) {
  return $c.has(e.tagName) || e.tabIndex !== -1;
}
const lt = /* @__PURE__ */ new WeakSet();
function ur(e) {
  return (t) => {
    t.key === "Enter" && e(t);
  };
}
function yn(e, t) {
  e.dispatchEvent(new PointerEvent("pointer" + t, { isPrimary: !0, bubbles: !0 }));
}
const Wc = (e, t) => {
  const n = e.currentTarget;
  if (!n)
    return;
  const s = ur(() => {
    if (lt.has(n))
      return;
    yn(n, "down");
    const r = ur(() => {
      yn(n, "up");
    }), o = () => yn(n, "cancel");
    n.addEventListener("keyup", r, t), n.addEventListener("blur", o, t);
  });
  n.addEventListener("keydown", s, t), n.addEventListener("blur", () => n.removeEventListener("keydown", s), t);
};
function dr(e) {
  return ys(e) && !oo();
}
function Hc(e, t, n = {}) {
  const [s, r, o] = ao(e, n), i = (a) => {
    const l = a.currentTarget;
    if (!dr(a) || lt.has(l))
      return;
    lt.add(l);
    const c = t(a), u = (m, g) => {
      window.removeEventListener("pointerup", d), window.removeEventListener("pointercancel", f), !(!dr(m) || !lt.has(l)) && (lt.delete(l), typeof c == "function" && c(m, { success: g }));
    }, d = (m) => {
      u(m, n.useGlobalTarget || lo(l, m.target));
    }, f = (m) => {
      u(m, !1);
    };
    window.addEventListener("pointerup", d, r), window.addEventListener("pointercancel", f, r);
  };
  return s.forEach((a) => {
    !Kc(a) && a.getAttribute("tabindex") === null && (a.tabIndex = 0), (n.useGlobalTarget ? window : a).addEventListener("pointerdown", i, r), a.addEventListener("focus", (c) => Wc(c, r), r);
  }), o;
}
function Gc(e) {
  return e === "x" || e === "y" ? ue[e] ? null : (ue[e] = !0, () => {
    ue[e] = !1;
  }) : ue.x || ue.y ? null : (ue.x = ue.y = !0, () => {
    ue.x = ue.y = !1;
  });
}
const co = /* @__PURE__ */ new Set([
  "width",
  "height",
  "top",
  "left",
  "right",
  "bottom",
  ...Qe
]);
let It;
function zc() {
  It = void 0;
}
const ye = {
  now: () => (It === void 0 && ye.set(G.isProcessing || $l.useManualTiming ? G.timestamp : performance.now()), It),
  set: (e) => {
    It = e, queueMicrotask(zc);
  }
};
function vs(e, t) {
  e.indexOf(t) === -1 && e.push(t);
}
function xs(e, t) {
  const n = e.indexOf(t);
  n > -1 && e.splice(n, 1);
}
class Ts {
  constructor() {
    this.subscriptions = [];
  }
  add(t) {
    return vs(this.subscriptions, t), () => xs(this.subscriptions, t);
  }
  notify(t, n, s) {
    const r = this.subscriptions.length;
    if (r)
      if (r === 1)
        this.subscriptions[0](t, n, s);
      else
        for (let o = 0; o < r; o++) {
          const i = this.subscriptions[o];
          i && i(t, n, s);
        }
  }
  getSize() {
    return this.subscriptions.length;
  }
  clear() {
    this.subscriptions.length = 0;
  }
}
function uo(e, t) {
  return t ? e * (1e3 / t) : 0;
}
const hr = 30, Yc = (e) => !isNaN(parseFloat(e));
class Xc {
  /**
   * @param init - The initiating value
   * @param config - Optional configuration options
   *
   * -  `transformer`: A function to transform incoming values with.
   *
   * @internal
   */
  constructor(t, n = {}) {
    this.version = "11.18.2", this.canTrackVelocity = null, this.events = {}, this.updateAndNotify = (s, r = !0) => {
      const o = ye.now();
      this.updatedAt !== o && this.setPrevFrameValue(), this.prev = this.current, this.setCurrent(s), this.current !== this.prev && this.events.change && this.events.change.notify(this.current), r && this.events.renderRequest && this.events.renderRequest.notify(this.current);
    }, this.hasAnimated = !1, this.setCurrent(t), this.owner = n.owner;
  }
  setCurrent(t) {
    this.current = t, this.updatedAt = ye.now(), this.canTrackVelocity === null && t !== void 0 && (this.canTrackVelocity = Yc(this.current));
  }
  setPrevFrameValue(t = this.current) {
    this.prevFrameValue = t, this.prevUpdatedAt = this.updatedAt;
  }
  /**
   * Adds a function that will be notified when the `MotionValue` is updated.
   *
   * It returns a function that, when called, will cancel the subscription.
   *
   * When calling `onChange` inside a React component, it should be wrapped with the
   * `useEffect` hook. As it returns an unsubscribe function, this should be returned
   * from the `useEffect` function to ensure you don't add duplicate subscribers..
   *
   * ```jsx
   * export const MyComponent = () => {
   *   const x = useMotionValue(0)
   *   const y = useMotionValue(0)
   *   const opacity = useMotionValue(1)
   *
   *   useEffect(() => {
   *     function updateOpacity() {
   *       const maxXY = Math.max(x.get(), y.get())
   *       const newOpacity = transform(maxXY, [0, 100], [1, 0])
   *       opacity.set(newOpacity)
   *     }
   *
   *     const unsubscribeX = x.on("change", updateOpacity)
   *     const unsubscribeY = y.on("change", updateOpacity)
   *
   *     return () => {
   *       unsubscribeX()
   *       unsubscribeY()
   *     }
   *   }, [])
   *
   *   return <motion.div style={{ x }} />
   * }
   * ```
   *
   * @param subscriber - A function that receives the latest value.
   * @returns A function that, when called, will cancel this subscription.
   *
   * @deprecated
   */
  onChange(t) {
    return process.env.NODE_ENV !== "production" && Jt(!1, 'value.onChange(callback) is deprecated. Switch to value.on("change", callback).'), this.on("change", t);
  }
  on(t, n) {
    this.events[t] || (this.events[t] = new Ts());
    const s = this.events[t].add(n);
    return t === "change" ? () => {
      s(), _.read(() => {
        this.events.change.getSize() || this.stop();
      });
    } : s;
  }
  clearListeners() {
    for (const t in this.events)
      this.events[t].clear();
  }
  /**
   * Attaches a passive effect to the `MotionValue`.
   *
   * @internal
   */
  attach(t, n) {
    this.passiveEffect = t, this.stopPassiveEffect = n;
  }
  /**
   * Sets the state of the `MotionValue`.
   *
   * @remarks
   *
   * ```jsx
   * const x = useMotionValue(0)
   * x.set(10)
   * ```
   *
   * @param latest - Latest value to set.
   * @param render - Whether to notify render subscribers. Defaults to `true`
   *
   * @public
   */
  set(t, n = !0) {
    !n || !this.passiveEffect ? this.updateAndNotify(t, n) : this.passiveEffect(t, this.updateAndNotify);
  }
  setWithVelocity(t, n, s) {
    this.set(n), this.prev = void 0, this.prevFrameValue = t, this.prevUpdatedAt = this.updatedAt - s;
  }
  /**
   * Set the state of the `MotionValue`, stopping any active animations,
   * effects, and resets velocity to `0`.
   */
  jump(t, n = !0) {
    this.updateAndNotify(t), this.prev = t, this.prevUpdatedAt = this.prevFrameValue = void 0, n && this.stop(), this.stopPassiveEffect && this.stopPassiveEffect();
  }
  /**
   * Returns the latest state of `MotionValue`
   *
   * @returns - The latest state of `MotionValue`
   *
   * @public
   */
  get() {
    return this.current;
  }
  /**
   * @public
   */
  getPrevious() {
    return this.prev;
  }
  /**
   * Returns the latest velocity of `MotionValue`
   *
   * @returns - The latest velocity of `MotionValue`. Returns `0` if the state is non-numerical.
   *
   * @public
   */
  getVelocity() {
    const t = ye.now();
    if (!this.canTrackVelocity || this.prevFrameValue === void 0 || t - this.updatedAt > hr)
      return 0;
    const n = Math.min(this.updatedAt - this.prevUpdatedAt, hr);
    return uo(parseFloat(this.current) - parseFloat(this.prevFrameValue), n);
  }
  /**
   * Registers a new animation to control this `MotionValue`. Only one
   * animation can drive a `MotionValue` at one time.
   *
   * ```jsx
   * value.start()
   * ```
   *
   * @param animation - A function that starts the provided animation
   *
   * @internal
   */
  start(t) {
    return this.stop(), new Promise((n) => {
      this.hasAnimated = !0, this.animation = t(n), this.events.animationStart && this.events.animationStart.notify();
    }).then(() => {
      this.events.animationComplete && this.events.animationComplete.notify(), this.clearAnimation();
    });
  }
  /**
   * Stop the currently active animation.
   *
   * @public
   */
  stop() {
    this.animation && (this.animation.stop(), this.events.animationCancel && this.events.animationCancel.notify()), this.clearAnimation();
  }
  /**
   * Returns `true` if this value is currently animating.
   *
   * @public
   */
  isAnimating() {
    return !!this.animation;
  }
  clearAnimation() {
    delete this.animation;
  }
  /**
   * Destroy and clean up subscribers to this `MotionValue`.
   *
   * The `MotionValue` hooks like `useMotionValue` and `useTransform` automatically
   * handle the lifecycle of the returned `MotionValue`, so this method is only necessary if you've manually
   * created a `MotionValue` via the `motionValue` function.
   *
   * @public
   */
  destroy() {
    this.clearListeners(), this.stop(), this.stopPassiveEffect && this.stopPassiveEffect();
  }
}
function vt(e, t) {
  return new Xc(e, t);
}
function qc(e, t, n) {
  e.hasValue(t) ? e.getValue(t).set(n) : e.addValue(t, vt(n));
}
function Zc(e, t) {
  const n = nn(e, t);
  let { transitionEnd: s = {}, transition: r = {}, ...o } = n || {};
  o = { ...o, ...s };
  for (const i in o) {
    const a = cc(o[i]);
    qc(e, i, a);
  }
}
function Jc(e) {
  return !!(Z(e) && e.add);
}
function Nn(e, t) {
  const n = e.getValue("willChange");
  if (Jc(n))
    return n.add(t);
}
function ho(e) {
  return e.props[Ui];
}
const fo = (e, t, n) => (((1 - 3 * n + 3 * t) * e + (3 * n - 6 * t)) * e + 3 * t) * e, Qc = 1e-7, eu = 12;
function tu(e, t, n, s, r) {
  let o, i, a = 0;
  do
    i = t + (n - t) / 2, o = fo(i, s, r) - e, o > 0 ? n = i : t = i;
  while (Math.abs(o) > Qc && ++a < eu);
  return i;
}
function Et(e, t, n, s) {
  if (e === t && n === s)
    return ee;
  const r = (o) => tu(o, 0, 1, e, n);
  return (o) => o === 0 || o === 1 ? o : fo(r(o), t, s);
}
const po = (e) => (t) => t <= 0.5 ? e(2 * t) / 2 : (2 - e(2 * (1 - t))) / 2, mo = (e) => (t) => 1 - e(1 - t), go = /* @__PURE__ */ Et(0.33, 1.53, 0.69, 0.99), bs = /* @__PURE__ */ mo(go), yo = /* @__PURE__ */ po(bs), vo = (e) => (e *= 2) < 1 ? 0.5 * bs(e) : 0.5 * (2 - Math.pow(2, -10 * (e - 1))), Es = (e) => 1 - Math.sin(Math.acos(e)), xo = mo(Es), To = po(Es), bo = (e) => /^0[^.\s]+$/u.test(e);
function nu(e) {
  return typeof e == "number" ? e === 0 : e !== null ? e === "none" || e === "0" || bo(e) : !0;
}
const dt = (e) => Math.round(e * 1e5) / 1e5, Ss = /-?(?:\d+(?:\.\d+)?|\.\d+)/gu;
function su(e) {
  return e == null;
}
const ru = /^(?:#[\da-f]{3,8}|(?:rgb|hsl)a?\((?:-?[\d.]+%?[,\s]+){2}-?[\d.]+%?\s*(?:[,/]\s*)?(?:\b\d+(?:\.\d+)?|\.\d+)?%?\))$/iu, ws = (e, t) => (n) => !!(typeof n == "string" && ru.test(n) && n.startsWith(e) || t && !su(n) && Object.prototype.hasOwnProperty.call(n, t)), Eo = (e, t, n) => (s) => {
  if (typeof s != "string")
    return s;
  const [r, o, i, a] = s.match(Ss);
  return {
    [e]: parseFloat(r),
    [t]: parseFloat(o),
    [n]: parseFloat(i),
    alpha: a !== void 0 ? parseFloat(a) : 1
  };
}, iu = (e) => be(0, 255, e), vn = {
  ...et,
  transform: (e) => Math.round(iu(e))
}, Me = {
  test: /* @__PURE__ */ ws("rgb", "red"),
  parse: /* @__PURE__ */ Eo("red", "green", "blue"),
  transform: ({ red: e, green: t, blue: n, alpha: s = 1 }) => "rgba(" + vn.transform(e) + ", " + vn.transform(t) + ", " + vn.transform(n) + ", " + dt(yt.transform(s)) + ")"
};
function ou(e) {
  let t = "", n = "", s = "", r = "";
  return e.length > 5 ? (t = e.substring(1, 3), n = e.substring(3, 5), s = e.substring(5, 7), r = e.substring(7, 9)) : (t = e.substring(1, 2), n = e.substring(2, 3), s = e.substring(3, 4), r = e.substring(4, 5), t += t, n += n, s += s, r += r), {
    red: parseInt(t, 16),
    green: parseInt(n, 16),
    blue: parseInt(s, 16),
    alpha: r ? parseInt(r, 16) / 255 : 1
  };
}
const Vn = {
  test: /* @__PURE__ */ ws("#"),
  parse: ou,
  transform: Me.transform
}, We = {
  test: /* @__PURE__ */ ws("hsl", "hue"),
  parse: /* @__PURE__ */ Eo("hue", "saturation", "lightness"),
  transform: ({ hue: e, saturation: t, lightness: n, alpha: s = 1 }) => "hsla(" + Math.round(e) + ", " + ge.transform(dt(t)) + ", " + ge.transform(dt(n)) + ", " + dt(yt.transform(s)) + ")"
}, q = {
  test: (e) => Me.test(e) || Vn.test(e) || We.test(e),
  parse: (e) => Me.test(e) ? Me.parse(e) : We.test(e) ? We.parse(e) : Vn.parse(e),
  transform: (e) => typeof e == "string" ? e : e.hasOwnProperty("red") ? Me.transform(e) : We.transform(e)
}, au = /(?:#[\da-f]{3,8}|(?:rgb|hsl)a?\((?:-?[\d.]+%?[,\s]+){2}-?[\d.]+%?\s*(?:[,/]\s*)?(?:\b\d+(?:\.\d+)?|\.\d+)?%?\))/giu;
function lu(e) {
  var t, n;
  return isNaN(e) && typeof e == "string" && (((t = e.match(Ss)) === null || t === void 0 ? void 0 : t.length) || 0) + (((n = e.match(au)) === null || n === void 0 ? void 0 : n.length) || 0) > 0;
}
const So = "number", wo = "color", cu = "var", uu = "var(", fr = "${}", du = /var\s*\(\s*--(?:[\w-]+\s*|[\w-]+\s*,(?:\s*[^)(\s]|\s*\((?:[^)(]|\([^)(]*\))*\))+\s*)\)|#[\da-f]{3,8}|(?:rgb|hsl)a?\((?:-?[\d.]+%?[,\s]+){2}-?[\d.]+%?\s*(?:[,/]\s*)?(?:\b\d+(?:\.\d+)?|\.\d+)?%?\)|-?(?:\d+(?:\.\d+)?|\.\d+)/giu;
function xt(e) {
  const t = e.toString(), n = [], s = {
    color: [],
    number: [],
    var: []
  }, r = [];
  let o = 0;
  const a = t.replace(du, (l) => (q.test(l) ? (s.color.push(o), r.push(wo), n.push(q.parse(l))) : l.startsWith(uu) ? (s.var.push(o), r.push(cu), n.push(l)) : (s.number.push(o), r.push(So), n.push(parseFloat(l))), ++o, fr)).split(fr);
  return { values: n, split: a, indexes: s, types: r };
}
function Ao(e) {
  return xt(e).values;
}
function Ro(e) {
  const { split: t, types: n } = xt(e), s = t.length;
  return (r) => {
    let o = "";
    for (let i = 0; i < s; i++)
      if (o += t[i], r[i] !== void 0) {
        const a = n[i];
        a === So ? o += dt(r[i]) : a === wo ? o += q.transform(r[i]) : o += r[i];
      }
    return o;
  };
}
const hu = (e) => typeof e == "number" ? 0 : e;
function fu(e) {
  const t = Ao(e);
  return Ro(e)(t.map(hu));
}
const Pe = {
  test: lu,
  parse: Ao,
  createTransformer: Ro,
  getAnimatableNone: fu
}, pu = /* @__PURE__ */ new Set(["brightness", "contrast", "saturate", "opacity"]);
function mu(e) {
  const [t, n] = e.slice(0, -1).split("(");
  if (t === "drop-shadow")
    return e;
  const [s] = n.match(Ss) || [];
  if (!s)
    return e;
  const r = n.replace(s, "");
  let o = pu.has(t) ? 1 : 0;
  return s !== n && (o *= 100), t + "(" + o + r + ")";
}
const gu = /\b([a-z-]*)\(.*?\)/gu, Mn = {
  ...Pe,
  getAnimatableNone: (e) => {
    const t = e.match(gu);
    return t ? t.map(mu).join(" ") : e;
  }
}, yu = {
  ...ls,
  // Color props
  color: q,
  backgroundColor: q,
  outlineColor: q,
  fill: q,
  stroke: q,
  // Border props
  borderColor: q,
  borderTopColor: q,
  borderRightColor: q,
  borderBottomColor: q,
  borderLeftColor: q,
  filter: Mn,
  WebkitFilter: Mn
}, As = (e) => yu[e];
function Po(e, t) {
  let n = As(e);
  return n !== Mn && (n = Pe), n.getAnimatableNone ? n.getAnimatableNone(t) : void 0;
}
const vu = /* @__PURE__ */ new Set(["auto", "none", "0"]);
function xu(e, t, n) {
  let s = 0, r;
  for (; s < e.length && !r; ) {
    const o = e[s];
    typeof o == "string" && !vu.has(o) && xt(o).values.length && (r = e[s]), s++;
  }
  if (r && n)
    for (const o of t)
      e[o] = Po(n, r);
}
const pr = (e) => e === et || e === A, mr = (e, t) => parseFloat(e.split(", ")[t]), gr = (e, t) => (n, { transform: s }) => {
  if (s === "none" || !s)
    return 0;
  const r = s.match(/^matrix3d\((.+)\)$/u);
  if (r)
    return mr(r[1], t);
  {
    const o = s.match(/^matrix\((.+)\)$/u);
    return o ? mr(o[1], e) : 0;
  }
}, Tu = /* @__PURE__ */ new Set(["x", "y", "z"]), bu = Qe.filter((e) => !Tu.has(e));
function Eu(e) {
  const t = [];
  return bu.forEach((n) => {
    const s = e.getValue(n);
    s !== void 0 && (t.push([n, s.get()]), s.set(n.startsWith("scale") ? 1 : 0));
  }), t;
}
const Ze = {
  // Dimensions
  width: ({ x: e }, { paddingLeft: t = "0", paddingRight: n = "0" }) => e.max - e.min - parseFloat(t) - parseFloat(n),
  height: ({ y: e }, { paddingTop: t = "0", paddingBottom: n = "0" }) => e.max - e.min - parseFloat(t) - parseFloat(n),
  top: (e, { top: t }) => parseFloat(t),
  left: (e, { left: t }) => parseFloat(t),
  bottom: ({ y: e }, { top: t }) => parseFloat(t) + (e.max - e.min),
  right: ({ x: e }, { left: t }) => parseFloat(t) + (e.max - e.min),
  // Transform
  x: gr(4, 13),
  y: gr(5, 14)
};
Ze.translateX = Ze.x;
Ze.translateY = Ze.y;
const Oe = /* @__PURE__ */ new Set();
let On = !1, Ln = !1;
function Co() {
  if (Ln) {
    const e = Array.from(Oe).filter((s) => s.needsMeasurement), t = new Set(e.map((s) => s.element)), n = /* @__PURE__ */ new Map();
    t.forEach((s) => {
      const r = Eu(s);
      r.length && (n.set(s, r), s.render());
    }), e.forEach((s) => s.measureInitialState()), t.forEach((s) => {
      s.render();
      const r = n.get(s);
      r && r.forEach(([o, i]) => {
        var a;
        (a = s.getValue(o)) === null || a === void 0 || a.set(i);
      });
    }), e.forEach((s) => s.measureEndState()), e.forEach((s) => {
      s.suspendedScrollY !== void 0 && window.scrollTo(0, s.suspendedScrollY);
    });
  }
  Ln = !1, On = !1, Oe.forEach((e) => e.complete()), Oe.clear();
}
function Do() {
  Oe.forEach((e) => {
    e.readKeyframes(), e.needsMeasurement && (Ln = !0);
  });
}
function Su() {
  Do(), Co();
}
class Rs {
  constructor(t, n, s, r, o, i = !1) {
    this.isComplete = !1, this.isAsync = !1, this.needsMeasurement = !1, this.isScheduled = !1, this.unresolvedKeyframes = [...t], this.onComplete = n, this.name = s, this.motionValue = r, this.element = o, this.isAsync = i;
  }
  scheduleResolve() {
    this.isScheduled = !0, this.isAsync ? (Oe.add(this), On || (On = !0, _.read(Do), _.resolveKeyframes(Co))) : (this.readKeyframes(), this.complete());
  }
  readKeyframes() {
    const { unresolvedKeyframes: t, name: n, element: s, motionValue: r } = this;
    for (let o = 0; o < t.length; o++)
      if (t[o] === null)
        if (o === 0) {
          const i = r == null ? void 0 : r.get(), a = t[t.length - 1];
          if (i !== void 0)
            t[0] = i;
          else if (s && n) {
            const l = s.readValue(n, a);
            l != null && (t[0] = l);
          }
          t[0] === void 0 && (t[0] = a), r && i === void 0 && r.set(t[0]);
        } else
          t[o] = t[o - 1];
  }
  setFinalKeyframe() {
  }
  measureInitialState() {
  }
  renderEndStyles() {
  }
  measureEndState() {
  }
  complete() {
    this.isComplete = !0, this.onComplete(this.unresolvedKeyframes, this.finalKeyframe), Oe.delete(this);
  }
  cancel() {
    this.isComplete || (this.isScheduled = !1, Oe.delete(this));
  }
  resume() {
    this.isComplete || this.scheduleResolve();
  }
}
const jo = (e) => /^-?(?:\d+(?:\.\d+)?|\.\d+)$/u.test(e), wu = (
  // eslint-disable-next-line redos-detector/no-unsafe-regex -- false positive, as it can match a lot of words
  /^var\(--(?:([\w-]+)|([\w-]+), ?([a-zA-Z\d ()%#.,-]+))\)/u
);
function Au(e) {
  const t = wu.exec(e);
  if (!t)
    return [,];
  const [, n, s, r] = t;
  return [`--${n ?? s}`, r];
}
const Ru = 4;
function ko(e, t, n = 1) {
  Ae(n <= Ru, `Max CSS variable fallback depth detected in property "${e}". This may indicate a circular fallback dependency.`);
  const [s, r] = Au(e);
  if (!s)
    return;
  const o = window.getComputedStyle(t).getPropertyValue(s);
  if (o) {
    const i = o.trim();
    return jo(i) ? parseFloat(i) : i;
  }
  return as(r) ? ko(r, t, n + 1) : r;
}
const No = (e) => (t) => t.test(e), Pu = {
  test: (e) => e === "auto",
  parse: (e) => e
}, Vo = [et, A, ge, we, mc, pc, Pu], yr = (e) => Vo.find(No(e));
class Mo extends Rs {
  constructor(t, n, s, r, o) {
    super(t, n, s, r, o, !0);
  }
  readKeyframes() {
    const { unresolvedKeyframes: t, element: n, name: s } = this;
    if (!n || !n.current)
      return;
    super.readKeyframes();
    for (let l = 0; l < t.length; l++) {
      let c = t[l];
      if (typeof c == "string" && (c = c.trim(), as(c))) {
        const u = ko(c, n.current);
        u !== void 0 && (t[l] = u), l === t.length - 1 && (this.finalKeyframe = c);
      }
    }
    if (this.resolveNoneKeyframes(), !co.has(s) || t.length !== 2)
      return;
    const [r, o] = t, i = yr(r), a = yr(o);
    if (i !== a)
      if (pr(i) && pr(a))
        for (let l = 0; l < t.length; l++) {
          const c = t[l];
          typeof c == "string" && (t[l] = parseFloat(c));
        }
      else
        this.needsMeasurement = !0;
  }
  resolveNoneKeyframes() {
    const { unresolvedKeyframes: t, name: n } = this, s = [];
    for (let r = 0; r < t.length; r++)
      nu(t[r]) && s.push(r);
    s.length && xu(t, s, n);
  }
  measureInitialState() {
    const { element: t, unresolvedKeyframes: n, name: s } = this;
    if (!t || !t.current)
      return;
    s === "height" && (this.suspendedScrollY = window.pageYOffset), this.measuredOrigin = Ze[s](t.measureViewportBox(), window.getComputedStyle(t.current)), n[0] = this.measuredOrigin;
    const r = n[n.length - 1];
    r !== void 0 && t.getValue(s, r).jump(r, !1);
  }
  measureEndState() {
    var t;
    const { element: n, name: s, unresolvedKeyframes: r } = this;
    if (!n || !n.current)
      return;
    const o = n.getValue(s);
    o && o.jump(this.measuredOrigin, !1);
    const i = r.length - 1, a = r[i];
    r[i] = Ze[s](n.measureViewportBox(), window.getComputedStyle(n.current)), a !== null && this.finalKeyframe === void 0 && (this.finalKeyframe = a), !((t = this.removedTransforms) === null || t === void 0) && t.length && this.removedTransforms.forEach(([l, c]) => {
      n.getValue(l).set(c);
    }), this.resolveNoneKeyframes();
  }
}
const vr = (e, t) => t === "zIndex" ? !1 : !!(typeof e == "number" || Array.isArray(e) || typeof e == "string" && // It's animatable if we have a string
(Pe.test(e) || e === "0") && // And it contains numbers and/or colors
!e.startsWith("url("));
function Cu(e) {
  const t = e[0];
  if (e.length === 1)
    return !0;
  for (let n = 0; n < e.length; n++)
    if (e[n] !== t)
      return !0;
}
function Du(e, t, n, s) {
  const r = e[0];
  if (r === null)
    return !1;
  if (t === "display" || t === "visibility")
    return !0;
  const o = e[e.length - 1], i = vr(r, t), a = vr(o, t);
  return Je(i === a, `You are trying to animate ${t} from "${r}" to "${o}". ${r} is not an animatable value - to enable this animation set ${r} to a value animatable to ${o} via the \`style\` property.`), !i || !a ? !1 : Cu(e) || (n === "spring" || ms(n)) && s;
}
const ju = (e) => e !== null;
function sn(e, { repeat: t, repeatType: n = "loop" }, s) {
  const r = e.filter(ju), o = t && n !== "loop" && t % 2 === 1 ? 0 : r.length - 1;
  return !o || s === void 0 ? r[o] : s;
}
const ku = 40;
class Oo {
  constructor({ autoplay: t = !0, delay: n = 0, type: s = "keyframes", repeat: r = 0, repeatDelay: o = 0, repeatType: i = "loop", ...a }) {
    this.isStopped = !1, this.hasAttemptedResolve = !1, this.createdAt = ye.now(), this.options = {
      autoplay: t,
      delay: n,
      type: s,
      repeat: r,
      repeatDelay: o,
      repeatType: i,
      ...a
    }, this.updateFinishedPromise();
  }
  /**
   * This method uses the createdAt and resolvedAt to calculate the
   * animation startTime. *Ideally*, we would use the createdAt time as t=0
   * as the following frame would then be the first frame of the animation in
   * progress, which would feel snappier.
   *
   * However, if there's a delay (main thread work) between the creation of
   * the animation and the first commited frame, we prefer to use resolvedAt
   * to avoid a sudden jump into the animation.
   */
  calcStartTime() {
    return this.resolvedAt ? this.resolvedAt - this.createdAt > ku ? this.resolvedAt : this.createdAt : this.createdAt;
  }
  /**
   * A getter for resolved data. If keyframes are not yet resolved, accessing
   * this.resolved will synchronously flush all pending keyframe resolvers.
   * This is a deoptimisation, but at its worst still batches read/writes.
   */
  get resolved() {
    return !this._resolved && !this.hasAttemptedResolve && Su(), this._resolved;
  }
  /**
   * A method to be called when the keyframes resolver completes. This method
   * will check if its possible to run the animation and, if not, skip it.
   * Otherwise, it will call initPlayback on the implementing class.
   */
  onKeyframesResolved(t, n) {
    this.resolvedAt = ye.now(), this.hasAttemptedResolve = !0;
    const { name: s, type: r, velocity: o, delay: i, onComplete: a, onUpdate: l, isGenerator: c } = this.options;
    if (!c && !Du(t, s, r, o))
      if (i)
        this.options.duration = 0;
      else {
        l && l(sn(t, this.options, n)), a && a(), this.resolveFinishedPromise();
        return;
      }
    const u = this.initPlayback(t, n);
    u !== !1 && (this._resolved = {
      keyframes: t,
      finalKeyframe: n,
      ...u
    }, this.onPostResolved());
  }
  onPostResolved() {
  }
  /**
   * Allows the returned animation to be awaited or promise-chained. Currently
   * resolves when the animation finishes at all but in a future update could/should
   * reject if its cancels.
   */
  then(t, n) {
    return this.currentFinishedPromise.then(t, n);
  }
  flatten() {
    this.options.type = "keyframes", this.options.ease = "linear";
  }
  updateFinishedPromise() {
    this.currentFinishedPromise = new Promise((t) => {
      this.resolveFinishedPromise = t;
    });
  }
}
const B = (e, t, n) => e + (t - e) * n;
function xn(e, t, n) {
  return n < 0 && (n += 1), n > 1 && (n -= 1), n < 1 / 6 ? e + (t - e) * 6 * n : n < 1 / 2 ? t : n < 2 / 3 ? e + (t - e) * (2 / 3 - n) * 6 : e;
}
function Nu({ hue: e, saturation: t, lightness: n, alpha: s }) {
  e /= 360, t /= 100, n /= 100;
  let r = 0, o = 0, i = 0;
  if (!t)
    r = o = i = n;
  else {
    const a = n < 0.5 ? n * (1 + t) : n + t - n * t, l = 2 * n - a;
    r = xn(l, a, e + 1 / 3), o = xn(l, a, e), i = xn(l, a, e - 1 / 3);
  }
  return {
    red: Math.round(r * 255),
    green: Math.round(o * 255),
    blue: Math.round(i * 255),
    alpha: s
  };
}
function Wt(e, t) {
  return (n) => n > 0 ? t : e;
}
const Tn = (e, t, n) => {
  const s = e * e, r = n * (t * t - s) + s;
  return r < 0 ? 0 : Math.sqrt(r);
}, Vu = [Vn, Me, We], Mu = (e) => Vu.find((t) => t.test(e));
function xr(e) {
  const t = Mu(e);
  if (Je(!!t, `'${e}' is not an animatable color. Use the equivalent color code instead.`), !t)
    return !1;
  let n = t.parse(e);
  return t === We && (n = Nu(n)), n;
}
const Tr = (e, t) => {
  const n = xr(e), s = xr(t);
  if (!n || !s)
    return Wt(e, t);
  const r = { ...n };
  return (o) => (r.red = Tn(n.red, s.red, o), r.green = Tn(n.green, s.green, o), r.blue = Tn(n.blue, s.blue, o), r.alpha = B(n.alpha, s.alpha, o), Me.transform(r));
}, Ou = (e, t) => (n) => t(e(n)), St = (...e) => e.reduce(Ou), In = /* @__PURE__ */ new Set(["none", "hidden"]);
function Lu(e, t) {
  return In.has(e) ? (n) => n <= 0 ? e : t : (n) => n >= 1 ? t : e;
}
function Iu(e, t) {
  return (n) => B(e, t, n);
}
function Ps(e) {
  return typeof e == "number" ? Iu : typeof e == "string" ? as(e) ? Wt : q.test(e) ? Tr : Bu : Array.isArray(e) ? Lo : typeof e == "object" ? q.test(e) ? Tr : _u : Wt;
}
function Lo(e, t) {
  const n = [...e], s = n.length, r = e.map((o, i) => Ps(o)(o, t[i]));
  return (o) => {
    for (let i = 0; i < s; i++)
      n[i] = r[i](o);
    return n;
  };
}
function _u(e, t) {
  const n = { ...e, ...t }, s = {};
  for (const r in n)
    e[r] !== void 0 && t[r] !== void 0 && (s[r] = Ps(e[r])(e[r], t[r]));
  return (r) => {
    for (const o in s)
      n[o] = s[o](r);
    return n;
  };
}
function Fu(e, t) {
  var n;
  const s = [], r = { color: 0, var: 0, number: 0 };
  for (let o = 0; o < t.values.length; o++) {
    const i = t.types[o], a = e.indexes[i][r[i]], l = (n = e.values[a]) !== null && n !== void 0 ? n : 0;
    s[o] = l, r[i]++;
  }
  return s;
}
const Bu = (e, t) => {
  const n = Pe.createTransformer(t), s = xt(e), r = xt(t);
  return s.indexes.var.length === r.indexes.var.length && s.indexes.color.length === r.indexes.color.length && s.indexes.number.length >= r.indexes.number.length ? In.has(e) && !r.values.length || In.has(t) && !s.values.length ? Lu(e, t) : St(Lo(Fu(s, r), r.values), n) : (Je(!0, `Complex values '${e}' and '${t}' too different to mix. Ensure all colors are of the same type, and that each contains the same quantity of number and color values. Falling back to instant transition.`), Wt(e, t));
};
function Io(e, t, n) {
  return typeof e == "number" && typeof t == "number" && typeof n == "number" ? B(e, t, n) : Ps(e)(e, t);
}
const Uu = 5;
function _o(e, t, n) {
  const s = Math.max(t - Uu, 0);
  return uo(n - e(s), t - s);
}
const F = {
  // Default spring physics
  stiffness: 100,
  damping: 10,
  mass: 1,
  velocity: 0,
  // Default duration/bounce-based options
  duration: 800,
  // in ms
  bounce: 0.3,
  visualDuration: 0.3,
  // in seconds
  // Rest thresholds
  restSpeed: {
    granular: 0.01,
    default: 2
  },
  restDelta: {
    granular: 5e-3,
    default: 0.5
  },
  // Limits
  minDuration: 0.01,
  // in seconds
  maxDuration: 10,
  // in seconds
  minDamping: 0.05,
  maxDamping: 1
}, bn = 1e-3;
function $u({ duration: e = F.duration, bounce: t = F.bounce, velocity: n = F.velocity, mass: s = F.mass }) {
  let r, o;
  Je(e <= /* @__PURE__ */ me(F.maxDuration), "Spring duration must be 10 seconds or less");
  let i = 1 - t;
  i = be(F.minDamping, F.maxDamping, i), e = be(F.minDuration, F.maxDuration, /* @__PURE__ */ xe(e)), i < 1 ? (r = (c) => {
    const u = c * i, d = u * e, f = u - n, m = _n(c, i), g = Math.exp(-d);
    return bn - f / m * g;
  }, o = (c) => {
    const d = c * i * e, f = d * n + n, m = Math.pow(i, 2) * Math.pow(c, 2) * e, g = Math.exp(-d), v = _n(Math.pow(c, 2), i);
    return (-r(c) + bn > 0 ? -1 : 1) * ((f - m) * g) / v;
  }) : (r = (c) => {
    const u = Math.exp(-c * e), d = (c - n) * e + 1;
    return -bn + u * d;
  }, o = (c) => {
    const u = Math.exp(-c * e), d = (n - c) * (e * e);
    return u * d;
  });
  const a = 5 / e, l = Wu(r, o, a);
  if (e = /* @__PURE__ */ me(e), isNaN(l))
    return {
      stiffness: F.stiffness,
      damping: F.damping,
      duration: e
    };
  {
    const c = Math.pow(l, 2) * s;
    return {
      stiffness: c,
      damping: i * 2 * Math.sqrt(s * c),
      duration: e
    };
  }
}
const Ku = 12;
function Wu(e, t, n) {
  let s = n;
  for (let r = 1; r < Ku; r++)
    s = s - e(s) / t(s);
  return s;
}
function _n(e, t) {
  return e * Math.sqrt(1 - t * t);
}
const Hu = ["duration", "bounce"], Gu = ["stiffness", "damping", "mass"];
function br(e, t) {
  return t.some((n) => e[n] !== void 0);
}
function zu(e) {
  let t = {
    velocity: F.velocity,
    stiffness: F.stiffness,
    damping: F.damping,
    mass: F.mass,
    isResolvedFromDuration: !1,
    ...e
  };
  if (!br(e, Gu) && br(e, Hu))
    if (e.visualDuration) {
      const n = e.visualDuration, s = 2 * Math.PI / (n * 1.2), r = s * s, o = 2 * be(0.05, 1, 1 - (e.bounce || 0)) * Math.sqrt(r);
      t = {
        ...t,
        mass: F.mass,
        stiffness: r,
        damping: o
      };
    } else {
      const n = $u(e);
      t = {
        ...t,
        ...n,
        mass: F.mass
      }, t.isResolvedFromDuration = !0;
    }
  return t;
}
function Fo(e = F.visualDuration, t = F.bounce) {
  const n = typeof e != "object" ? {
    visualDuration: e,
    keyframes: [0, 1],
    bounce: t
  } : e;
  let { restSpeed: s, restDelta: r } = n;
  const o = n.keyframes[0], i = n.keyframes[n.keyframes.length - 1], a = { done: !1, value: o }, { stiffness: l, damping: c, mass: u, duration: d, velocity: f, isResolvedFromDuration: m } = zu({
    ...n,
    velocity: -/* @__PURE__ */ xe(n.velocity || 0)
  }), g = f || 0, v = c / (2 * Math.sqrt(l * u)), T = i - o, x = /* @__PURE__ */ xe(Math.sqrt(l / u)), b = Math.abs(T) < 5;
  s || (s = b ? F.restSpeed.granular : F.restSpeed.default), r || (r = b ? F.restDelta.granular : F.restDelta.default);
  let w;
  if (v < 1) {
    const S = _n(x, v);
    w = (j) => {
      const O = Math.exp(-v * x * j);
      return i - O * ((g + v * x * T) / S * Math.sin(S * j) + T * Math.cos(S * j));
    };
  } else if (v === 1)
    w = (S) => i - Math.exp(-x * S) * (T + (g + x * T) * S);
  else {
    const S = x * Math.sqrt(v * v - 1);
    w = (j) => {
      const O = Math.exp(-v * x * j), C = Math.min(S * j, 300);
      return i - O * ((g + v * x * T) * Math.sinh(C) + S * T * Math.cosh(C)) / S;
    };
  }
  const P = {
    calculatedDuration: m && d || null,
    next: (S) => {
      const j = w(S);
      if (m)
        a.done = S >= d;
      else {
        let O = 0;
        v < 1 && (O = S === 0 ? /* @__PURE__ */ me(g) : _o(w, S, j));
        const C = Math.abs(O) <= s, D = Math.abs(i - j) <= r;
        a.done = C && D;
      }
      return a.value = a.done ? i : j, a;
    },
    toString: () => {
      const S = Math.min(no(P), jn), j = so((O) => P.next(S * O).value, S, 30);
      return S + "ms " + j;
    }
  };
  return P;
}
function Er({ keyframes: e, velocity: t = 0, power: n = 0.8, timeConstant: s = 325, bounceDamping: r = 10, bounceStiffness: o = 500, modifyTarget: i, min: a, max: l, restDelta: c = 0.5, restSpeed: u }) {
  const d = e[0], f = {
    done: !1,
    value: d
  }, m = (C) => a !== void 0 && C < a || l !== void 0 && C > l, g = (C) => a === void 0 ? l : l === void 0 || Math.abs(a - C) < Math.abs(l - C) ? a : l;
  let v = n * t;
  const T = d + v, x = i === void 0 ? T : i(T);
  x !== T && (v = x - d);
  const b = (C) => -v * Math.exp(-C / s), w = (C) => x + b(C), P = (C) => {
    const D = b(C), V = w(C);
    f.done = Math.abs(D) <= c, f.value = f.done ? x : V;
  };
  let S, j;
  const O = (C) => {
    m(f.value) && (S = C, j = Fo({
      keyframes: [f.value, g(f.value)],
      velocity: _o(w, C, f.value),
      // TODO: This should be passing * 1000
      damping: r,
      stiffness: o,
      restDelta: c,
      restSpeed: u
    }));
  };
  return O(0), {
    calculatedDuration: null,
    next: (C) => {
      let D = !1;
      return !j && S === void 0 && (D = !0, P(C), O(C)), S !== void 0 && C >= S ? j.next(C - S) : (!D && P(C), f);
    }
  };
}
const Yu = /* @__PURE__ */ Et(0.42, 0, 1, 1), Xu = /* @__PURE__ */ Et(0, 0, 0.58, 1), Bo = /* @__PURE__ */ Et(0.42, 0, 0.58, 1), qu = (e) => Array.isArray(e) && typeof e[0] != "number", Sr = {
  linear: ee,
  easeIn: Yu,
  easeInOut: Bo,
  easeOut: Xu,
  circIn: Es,
  circInOut: To,
  circOut: xo,
  backIn: bs,
  backInOut: yo,
  backOut: go,
  anticipate: vo
}, wr = (e) => {
  if (gs(e)) {
    Ae(e.length === 4, "Cubic bezier arrays must contain four numerical values.");
    const [t, n, s, r] = e;
    return Et(t, n, s, r);
  } else if (typeof e == "string")
    return Ae(Sr[e] !== void 0, `Invalid easing type '${e}'`), Sr[e];
  return e;
};
function Zu(e, t, n) {
  const s = [], r = n || Io, o = e.length - 1;
  for (let i = 0; i < o; i++) {
    let a = r(e[i], e[i + 1]);
    if (t) {
      const l = Array.isArray(t) ? t[i] || ee : t;
      a = St(l, a);
    }
    s.push(a);
  }
  return s;
}
function Ju(e, t, { clamp: n = !0, ease: s, mixer: r } = {}) {
  const o = e.length;
  if (Ae(o === t.length, "Both input and output ranges must be the same length"), o === 1)
    return () => t[0];
  if (o === 2 && t[0] === t[1])
    return () => t[1];
  const i = e[0] === e[1];
  e[0] > e[o - 1] && (e = [...e].reverse(), t = [...t].reverse());
  const a = Zu(t, s, r), l = a.length, c = (u) => {
    if (i && u < e[0])
      return t[0];
    let d = 0;
    if (l > 1)
      for (; d < e.length - 2 && !(u < e[d + 1]); d++)
        ;
    const f = /* @__PURE__ */ Xe(e[d], e[d + 1], u);
    return a[d](f);
  };
  return n ? (u) => c(be(e[0], e[o - 1], u)) : c;
}
function Qu(e, t) {
  const n = e[e.length - 1];
  for (let s = 1; s <= t; s++) {
    const r = /* @__PURE__ */ Xe(0, t, s);
    e.push(B(n, 1, r));
  }
}
function ed(e) {
  const t = [0];
  return Qu(t, e.length - 1), t;
}
function td(e, t) {
  return e.map((n) => n * t);
}
function nd(e, t) {
  return e.map(() => t || Bo).splice(0, e.length - 1);
}
function Ht({ duration: e = 300, keyframes: t, times: n, ease: s = "easeInOut" }) {
  const r = qu(s) ? s.map(wr) : wr(s), o = {
    done: !1,
    value: t[0]
  }, i = td(
    // Only use the provided offsets if they're the correct length
    // TODO Maybe we should warn here if there's a length mismatch
    n && n.length === t.length ? n : ed(t),
    e
  ), a = Ju(i, t, {
    ease: Array.isArray(r) ? r : nd(t, r)
  });
  return {
    calculatedDuration: e,
    next: (l) => (o.value = a(l), o.done = l >= e, o)
  };
}
const sd = (e) => {
  const t = ({ timestamp: n }) => e(n);
  return {
    start: () => _.update(t, !0),
    stop: () => Re(t),
    /**
     * If we're processing this frame we can use the
     * framelocked timestamp to keep things in sync.
     */
    now: () => G.isProcessing ? G.timestamp : ye.now()
  };
}, rd = {
  decay: Er,
  inertia: Er,
  tween: Ht,
  keyframes: Ht,
  spring: Fo
}, id = (e) => e / 100;
class Cs extends Oo {
  constructor(t) {
    super(t), this.holdTime = null, this.cancelTime = null, this.currentTime = 0, this.playbackSpeed = 1, this.pendingPlayState = "running", this.startTime = null, this.state = "idle", this.stop = () => {
      if (this.resolver.cancel(), this.isStopped = !0, this.state === "idle")
        return;
      this.teardown();
      const { onStop: l } = this.options;
      l && l();
    };
    const { name: n, motionValue: s, element: r, keyframes: o } = this.options, i = (r == null ? void 0 : r.KeyframeResolver) || Rs, a = (l, c) => this.onKeyframesResolved(l, c);
    this.resolver = new i(o, a, n, s, r), this.resolver.scheduleResolve();
  }
  flatten() {
    super.flatten(), this._resolved && Object.assign(this._resolved, this.initPlayback(this._resolved.keyframes));
  }
  initPlayback(t) {
    const { type: n = "keyframes", repeat: s = 0, repeatDelay: r = 0, repeatType: o, velocity: i = 0 } = this.options, a = ms(n) ? n : rd[n] || Ht;
    let l, c;
    a !== Ht && typeof t[0] != "number" && (process.env.NODE_ENV !== "production" && Ae(t.length === 2, `Only two keyframes currently supported with spring and inertia animations. Trying to animate ${t}`), l = St(id, Io(t[0], t[1])), t = [0, 100]);
    const u = a({ ...this.options, keyframes: t });
    o === "mirror" && (c = a({
      ...this.options,
      keyframes: [...t].reverse(),
      velocity: -i
    })), u.calculatedDuration === null && (u.calculatedDuration = no(u));
    const { calculatedDuration: d } = u, f = d + r, m = f * (s + 1) - r;
    return {
      generator: u,
      mirroredGenerator: c,
      mapPercentToKeyframes: l,
      calculatedDuration: d,
      resolvedDuration: f,
      totalDuration: m
    };
  }
  onPostResolved() {
    const { autoplay: t = !0 } = this.options;
    this.play(), this.pendingPlayState === "paused" || !t ? this.pause() : this.state = this.pendingPlayState;
  }
  tick(t, n = !1) {
    const { resolved: s } = this;
    if (!s) {
      const { keyframes: C } = this.options;
      return { done: !0, value: C[C.length - 1] };
    }
    const { finalKeyframe: r, generator: o, mirroredGenerator: i, mapPercentToKeyframes: a, keyframes: l, calculatedDuration: c, totalDuration: u, resolvedDuration: d } = s;
    if (this.startTime === null)
      return o.next(0);
    const { delay: f, repeat: m, repeatType: g, repeatDelay: v, onUpdate: T } = this.options;
    this.speed > 0 ? this.startTime = Math.min(this.startTime, t) : this.speed < 0 && (this.startTime = Math.min(t - u / this.speed, this.startTime)), n ? this.currentTime = t : this.holdTime !== null ? this.currentTime = this.holdTime : this.currentTime = Math.round(t - this.startTime) * this.speed;
    const x = this.currentTime - f * (this.speed >= 0 ? 1 : -1), b = this.speed >= 0 ? x < 0 : x > u;
    this.currentTime = Math.max(x, 0), this.state === "finished" && this.holdTime === null && (this.currentTime = u);
    let w = this.currentTime, P = o;
    if (m) {
      const C = Math.min(this.currentTime, u) / d;
      let D = Math.floor(C), V = C % 1;
      !V && C >= 1 && (V = 1), V === 1 && D--, D = Math.min(D, m + 1), !!(D % 2) && (g === "reverse" ? (V = 1 - V, v && (V -= v / d)) : g === "mirror" && (P = i)), w = be(0, 1, V) * d;
    }
    const S = b ? { done: !1, value: l[0] } : P.next(w);
    a && (S.value = a(S.value));
    let { done: j } = S;
    !b && c !== null && (j = this.speed >= 0 ? this.currentTime >= u : this.currentTime <= 0);
    const O = this.holdTime === null && (this.state === "finished" || this.state === "running" && j);
    return O && r !== void 0 && (S.value = sn(l, this.options, r)), T && T(S.value), O && this.finish(), S;
  }
  get duration() {
    const { resolved: t } = this;
    return t ? /* @__PURE__ */ xe(t.calculatedDuration) : 0;
  }
  get time() {
    return /* @__PURE__ */ xe(this.currentTime);
  }
  set time(t) {
    t = /* @__PURE__ */ me(t), this.currentTime = t, this.holdTime !== null || this.speed === 0 ? this.holdTime = t : this.driver && (this.startTime = this.driver.now() - t / this.speed);
  }
  get speed() {
    return this.playbackSpeed;
  }
  set speed(t) {
    const n = this.playbackSpeed !== t;
    this.playbackSpeed = t, n && (this.time = /* @__PURE__ */ xe(this.currentTime));
  }
  play() {
    if (this.resolver.isScheduled || this.resolver.resume(), !this._resolved) {
      this.pendingPlayState = "running";
      return;
    }
    if (this.isStopped)
      return;
    const { driver: t = sd, onPlay: n, startTime: s } = this.options;
    this.driver || (this.driver = t((o) => this.tick(o))), n && n();
    const r = this.driver.now();
    this.holdTime !== null ? this.startTime = r - this.holdTime : this.startTime ? this.state === "finished" && (this.startTime = r) : this.startTime = s ?? this.calcStartTime(), this.state === "finished" && this.updateFinishedPromise(), this.cancelTime = this.startTime, this.holdTime = null, this.state = "running", this.driver.start();
  }
  pause() {
    var t;
    if (!this._resolved) {
      this.pendingPlayState = "paused";
      return;
    }
    this.state = "paused", this.holdTime = (t = this.currentTime) !== null && t !== void 0 ? t : 0;
  }
  complete() {
    this.state !== "running" && this.play(), this.pendingPlayState = this.state = "finished", this.holdTime = null;
  }
  finish() {
    this.teardown(), this.state = "finished";
    const { onComplete: t } = this.options;
    t && t();
  }
  cancel() {
    this.cancelTime !== null && this.tick(this.cancelTime), this.teardown(), this.updateFinishedPromise();
  }
  teardown() {
    this.state = "idle", this.stopDriver(), this.resolveFinishedPromise(), this.updateFinishedPromise(), this.startTime = this.cancelTime = null, this.resolver.cancel();
  }
  stopDriver() {
    this.driver && (this.driver.stop(), this.driver = void 0);
  }
  sample(t) {
    return this.startTime = 0, this.tick(t, !0);
  }
}
const od = /* @__PURE__ */ new Set([
  "opacity",
  "clipPath",
  "filter",
  "transform"
  // TODO: Can be accelerated but currently disabled until https://issues.chromium.org/issues/41491098 is resolved
  // or until we implement support for linear() easing.
  // "background-color"
]);
function ad(e, t, n, { delay: s = 0, duration: r = 300, repeat: o = 0, repeatType: i = "loop", ease: a = "easeInOut", times: l } = {}) {
  const c = { [t]: n };
  l && (c.offset = l);
  const u = io(a, r);
  return Array.isArray(u) && (c.easing = u), e.animate(c, {
    delay: s,
    duration: r,
    easing: Array.isArray(u) ? "linear" : u,
    fill: "both",
    iterations: o + 1,
    direction: i === "reverse" ? "alternate" : "normal"
  });
}
const ld = /* @__PURE__ */ es(() => Object.hasOwnProperty.call(Element.prototype, "animate")), Gt = 10, cd = 2e4;
function ud(e) {
  return ms(e.type) || e.type === "spring" || !ro(e.ease);
}
function dd(e, t) {
  const n = new Cs({
    ...t,
    keyframes: e,
    repeat: 0,
    delay: 0,
    isGenerator: !0
  });
  let s = { done: !1, value: e[0] };
  const r = [];
  let o = 0;
  for (; !s.done && o < cd; )
    s = n.sample(o), r.push(s.value), o += Gt;
  return {
    times: void 0,
    keyframes: r,
    duration: o - Gt,
    ease: "linear"
  };
}
const Uo = {
  anticipate: vo,
  backInOut: yo,
  circInOut: To
};
function hd(e) {
  return e in Uo;
}
class Ar extends Oo {
  constructor(t) {
    super(t);
    const { name: n, motionValue: s, element: r, keyframes: o } = this.options;
    this.resolver = new Mo(o, (i, a) => this.onKeyframesResolved(i, a), n, s, r), this.resolver.scheduleResolve();
  }
  initPlayback(t, n) {
    let { duration: s = 300, times: r, ease: o, type: i, motionValue: a, name: l, startTime: c } = this.options;
    if (!a.owner || !a.owner.current)
      return !1;
    if (typeof o == "string" && Kt() && hd(o) && (o = Uo[o]), ud(this.options)) {
      const { onComplete: d, onUpdate: f, motionValue: m, element: g, ...v } = this.options, T = dd(t, v);
      t = T.keyframes, t.length === 1 && (t[1] = t[0]), s = T.duration, r = T.times, o = T.ease, i = "keyframes";
    }
    const u = ad(a.owner.current, l, t, { ...this.options, duration: s, times: r, ease: o });
    return u.startTime = c ?? this.calcStartTime(), this.pendingTimeline ? (lr(u, this.pendingTimeline), this.pendingTimeline = void 0) : u.onfinish = () => {
      const { onComplete: d } = this.options;
      a.set(sn(t, this.options, n)), d && d(), this.cancel(), this.resolveFinishedPromise();
    }, {
      animation: u,
      duration: s,
      times: r,
      type: i,
      ease: o,
      keyframes: t
    };
  }
  get duration() {
    const { resolved: t } = this;
    if (!t)
      return 0;
    const { duration: n } = t;
    return /* @__PURE__ */ xe(n);
  }
  get time() {
    const { resolved: t } = this;
    if (!t)
      return 0;
    const { animation: n } = t;
    return /* @__PURE__ */ xe(n.currentTime || 0);
  }
  set time(t) {
    const { resolved: n } = this;
    if (!n)
      return;
    const { animation: s } = n;
    s.currentTime = /* @__PURE__ */ me(t);
  }
  get speed() {
    const { resolved: t } = this;
    if (!t)
      return 1;
    const { animation: n } = t;
    return n.playbackRate;
  }
  set speed(t) {
    const { resolved: n } = this;
    if (!n)
      return;
    const { animation: s } = n;
    s.playbackRate = t;
  }
  get state() {
    const { resolved: t } = this;
    if (!t)
      return "idle";
    const { animation: n } = t;
    return n.playState;
  }
  get startTime() {
    const { resolved: t } = this;
    if (!t)
      return null;
    const { animation: n } = t;
    return n.startTime;
  }
  /**
   * Replace the default DocumentTimeline with another AnimationTimeline.
   * Currently used for scroll animations.
   */
  attachTimeline(t) {
    if (!this._resolved)
      this.pendingTimeline = t;
    else {
      const { resolved: n } = this;
      if (!n)
        return ee;
      const { animation: s } = n;
      lr(s, t);
    }
    return ee;
  }
  play() {
    if (this.isStopped)
      return;
    const { resolved: t } = this;
    if (!t)
      return;
    const { animation: n } = t;
    n.playState === "finished" && this.updateFinishedPromise(), n.play();
  }
  pause() {
    const { resolved: t } = this;
    if (!t)
      return;
    const { animation: n } = t;
    n.pause();
  }
  stop() {
    if (this.resolver.cancel(), this.isStopped = !0, this.state === "idle")
      return;
    this.resolveFinishedPromise(), this.updateFinishedPromise();
    const { resolved: t } = this;
    if (!t)
      return;
    const { animation: n, keyframes: s, duration: r, type: o, ease: i, times: a } = t;
    if (n.playState === "idle" || n.playState === "finished")
      return;
    if (this.time) {
      const { motionValue: c, onUpdate: u, onComplete: d, element: f, ...m } = this.options, g = new Cs({
        ...m,
        keyframes: s,
        duration: r,
        type: o,
        ease: i,
        times: a,
        isGenerator: !0
      }), v = /* @__PURE__ */ me(this.time);
      c.setWithVelocity(g.sample(v - Gt).value, g.sample(v).value, Gt);
    }
    const { onStop: l } = this.options;
    l && l(), this.cancel();
  }
  complete() {
    const { resolved: t } = this;
    t && t.animation.finish();
  }
  cancel() {
    const { resolved: t } = this;
    t && t.animation.cancel();
  }
  static supports(t) {
    const { motionValue: n, name: s, repeatDelay: r, repeatType: o, damping: i, type: a } = t;
    if (!n || !n.owner || !(n.owner.current instanceof HTMLElement))
      return !1;
    const { onUpdate: l, transformTemplate: c } = n.owner.getProps();
    return ld() && s && od.has(s) && /**
     * If we're outputting values to onUpdate then we can't use WAAPI as there's
     * no way to read the value from WAAPI every frame.
     */
    !l && !c && !r && o !== "mirror" && i !== 0 && a !== "inertia";
  }
}
const fd = {
  type: "spring",
  stiffness: 500,
  damping: 25,
  restSpeed: 10
}, pd = (e) => ({
  type: "spring",
  stiffness: 550,
  damping: e === 0 ? 2 * Math.sqrt(550) : 30,
  restSpeed: 10
}), md = {
  type: "keyframes",
  duration: 0.8
}, gd = {
  type: "keyframes",
  ease: [0.25, 0.1, 0.35, 1],
  duration: 0.3
}, yd = (e, { keyframes: t }) => t.length > 2 ? md : Ie.has(e) ? e.startsWith("scale") ? pd(t[1]) : fd : gd;
function vd({ when: e, delay: t, delayChildren: n, staggerChildren: s, staggerDirection: r, repeat: o, repeatType: i, repeatDelay: a, from: l, elapsed: c, ...u }) {
  return !!Object.keys(u).length;
}
const Ds = (e, t, n, s = {}, r, o) => (i) => {
  const a = ps(s, e) || {}, l = a.delay || s.delay || 0;
  let { elapsed: c = 0 } = s;
  c = c - /* @__PURE__ */ me(l);
  let u = {
    keyframes: Array.isArray(n) ? n : [null, n],
    ease: "easeOut",
    velocity: t.getVelocity(),
    ...a,
    delay: -c,
    onUpdate: (f) => {
      t.set(f), a.onUpdate && a.onUpdate(f);
    },
    onComplete: () => {
      i(), a.onComplete && a.onComplete();
    },
    name: e,
    motionValue: t,
    element: o ? void 0 : r
  };
  vd(a) || (u = {
    ...u,
    ...yd(e, u)
  }), u.duration && (u.duration = /* @__PURE__ */ me(u.duration)), u.repeatDelay && (u.repeatDelay = /* @__PURE__ */ me(u.repeatDelay)), u.from !== void 0 && (u.keyframes[0] = u.from);
  let d = !1;
  if ((u.type === !1 || u.duration === 0 && !u.repeatDelay) && (u.duration = 0, u.delay === 0 && (d = !0)), d && !o && t.get() !== void 0) {
    const f = sn(u.keyframes, a);
    if (f !== void 0)
      return _.update(() => {
        u.onUpdate(f), u.onComplete();
      }), new Ic([]);
  }
  return !o && Ar.supports(u) ? new Ar(u) : new Cs(u);
};
function xd({ protectedKeys: e, needsAnimating: t }, n) {
  const s = e.hasOwnProperty(n) && t[n] !== !0;
  return t[n] = !1, s;
}
function $o(e, t, { delay: n = 0, transitionOverride: s, type: r } = {}) {
  var o;
  let { transition: i = e.getDefaultTransition(), transitionEnd: a, ...l } = t;
  s && (i = s);
  const c = [], u = r && e.animationState && e.animationState.getState()[r];
  for (const d in l) {
    const f = e.getValue(d, (o = e.latestValues[d]) !== null && o !== void 0 ? o : null), m = l[d];
    if (m === void 0 || u && xd(u, d))
      continue;
    const g = {
      delay: n,
      ...ps(i || {}, d)
    };
    let v = !1;
    if (window.MotionHandoffAnimation) {
      const x = ho(e);
      if (x) {
        const b = window.MotionHandoffAnimation(x, d, _);
        b !== null && (g.startTime = b, v = !0);
      }
    }
    Nn(e, d), f.start(Ds(d, f, m, e.shouldReduceMotion && co.has(d) ? { type: !1 } : g, e, v));
    const T = f.animation;
    T && c.push(T);
  }
  return a && Promise.all(c).then(() => {
    _.update(() => {
      a && Zc(e, a);
    });
  }), c;
}
function Fn(e, t, n = {}) {
  var s;
  const r = nn(e, t, n.type === "exit" ? (s = e.presenceContext) === null || s === void 0 ? void 0 : s.custom : void 0);
  let { transition: o = e.getDefaultTransition() || {} } = r || {};
  n.transitionOverride && (o = n.transitionOverride);
  const i = r ? () => Promise.all($o(e, r, n)) : () => Promise.resolve(), a = e.variantChildren && e.variantChildren.size ? (c = 0) => {
    const { delayChildren: u = 0, staggerChildren: d, staggerDirection: f } = o;
    return Td(e, t, u + c, d, f, n);
  } : () => Promise.resolve(), { when: l } = o;
  if (l) {
    const [c, u] = l === "beforeChildren" ? [i, a] : [a, i];
    return c().then(() => u());
  } else
    return Promise.all([i(), a(n.delay)]);
}
function Td(e, t, n = 0, s = 0, r = 1, o) {
  const i = [], a = (e.variantChildren.size - 1) * s, l = r === 1 ? (c = 0) => c * s : (c = 0) => a - c * s;
  return Array.from(e.variantChildren).sort(bd).forEach((c, u) => {
    c.notify("AnimationStart", t), i.push(Fn(c, t, {
      ...o,
      delay: n + l(u)
    }).then(() => c.notify("AnimationComplete", t)));
  }), Promise.all(i);
}
function bd(e, t) {
  return e.sortNodePosition(t);
}
function Ed(e, t, n = {}) {
  e.notify("AnimationStart", t);
  let s;
  if (Array.isArray(t)) {
    const r = t.map((o) => Fn(e, o, n));
    s = Promise.all(r);
  } else if (typeof t == "string")
    s = Fn(e, t, n);
  else {
    const r = typeof t == "function" ? nn(e, t, n.custom) : t;
    s = Promise.all($o(e, r, n));
  }
  return s.then(() => {
    e.notify("AnimationComplete", t);
  });
}
const Sd = ns.length;
function Ko(e) {
  if (!e)
    return;
  if (!e.isControllingVariants) {
    const n = e.parent ? Ko(e.parent) || {} : {};
    return e.props.initial !== void 0 && (n.initial = e.props.initial), n;
  }
  const t = {};
  for (let n = 0; n < Sd; n++) {
    const s = ns[n], r = e.props[s];
    (gt(r) || r === !1) && (t[s] = r);
  }
  return t;
}
const wd = [...ts].reverse(), Ad = ts.length;
function Rd(e) {
  return (t) => Promise.all(t.map(({ animation: n, options: s }) => Ed(e, n, s)));
}
function Pd(e) {
  let t = Rd(e), n = Rr(), s = !0;
  const r = (l) => (c, u) => {
    var d;
    const f = nn(e, u, l === "exit" ? (d = e.presenceContext) === null || d === void 0 ? void 0 : d.custom : void 0);
    if (f) {
      const { transition: m, transitionEnd: g, ...v } = f;
      c = { ...c, ...v, ...g };
    }
    return c;
  };
  function o(l) {
    t = l(e);
  }
  function i(l) {
    const { props: c } = e, u = Ko(e.parent) || {}, d = [], f = /* @__PURE__ */ new Set();
    let m = {}, g = 1 / 0;
    for (let T = 0; T < Ad; T++) {
      const x = wd[T], b = n[x], w = c[x] !== void 0 ? c[x] : u[x], P = gt(w), S = x === l ? b.isActive : null;
      S === !1 && (g = T);
      let j = w === u[x] && w !== c[x] && P;
      if (j && s && e.manuallyAnimateOnMount && (j = !1), b.protectedKeys = { ...m }, // If it isn't active and hasn't *just* been set as inactive
      !b.isActive && S === null || // If we didn't and don't have any defined prop for this animation type
      !w && !b.prevProp || // Or if the prop doesn't define an animation
      en(w) || typeof w == "boolean")
        continue;
      const O = Cd(b.prevProp, w);
      let C = O || // If we're making this variant active, we want to always make it active
      x === l && b.isActive && !j && P || // If we removed a higher-priority variant (i is in reverse order)
      T > g && P, D = !1;
      const V = Array.isArray(w) ? w : [w];
      let te = V.reduce(r(x), {});
      S === !1 && (te = {});
      const { prevResolvedValues: Ee = {} } = b, fe = {
        ...Ee,
        ...te
      }, pe = (H) => {
        C = !0, f.has(H) && (D = !0, f.delete(H)), b.needsAnimating[H] = !0;
        const ie = e.getValue(H);
        ie && (ie.liveStyle = !1);
      };
      for (const H in fe) {
        const ie = te[H], Fe = Ee[H];
        if (m.hasOwnProperty(H))
          continue;
        let Be = !1;
        Dn(ie) && Dn(Fe) ? Be = !to(ie, Fe) : Be = ie !== Fe, Be ? ie != null ? pe(H) : f.add(H) : ie !== void 0 && f.has(H) ? pe(H) : b.protectedKeys[H] = !0;
      }
      b.prevProp = w, b.prevResolvedValues = te, b.isActive && (m = { ...m, ...te }), s && e.blockInitialAnimation && (C = !1), C && (!(j && O) || D) && d.push(...V.map((H) => ({
        animation: H,
        options: { type: x }
      })));
    }
    if (f.size) {
      const T = {};
      f.forEach((x) => {
        const b = e.getBaseTarget(x), w = e.getValue(x);
        w && (w.liveStyle = !0), T[x] = b ?? null;
      }), d.push({ animation: T });
    }
    let v = !!d.length;
    return s && (c.initial === !1 || c.initial === c.animate) && !e.manuallyAnimateOnMount && (v = !1), s = !1, v ? t(d) : Promise.resolve();
  }
  function a(l, c) {
    var u;
    if (n[l].isActive === c)
      return Promise.resolve();
    (u = e.variantChildren) === null || u === void 0 || u.forEach((f) => {
      var m;
      return (m = f.animationState) === null || m === void 0 ? void 0 : m.setActive(l, c);
    }), n[l].isActive = c;
    const d = i(l);
    for (const f in n)
      n[f].protectedKeys = {};
    return d;
  }
  return {
    animateChanges: i,
    setActive: a,
    setAnimateFunction: o,
    getState: () => n,
    reset: () => {
      n = Rr(), s = !0;
    }
  };
}
function Cd(e, t) {
  return typeof t == "string" ? t !== e : Array.isArray(t) ? !to(t, e) : !1;
}
function je(e = !1) {
  return {
    isActive: e,
    protectedKeys: {},
    needsAnimating: {},
    prevResolvedValues: {}
  };
}
function Rr() {
  return {
    animate: je(!0),
    whileInView: je(),
    whileHover: je(),
    whileTap: je(),
    whileDrag: je(),
    whileFocus: je(),
    exit: je()
  };
}
class Ce {
  constructor(t) {
    this.isMounted = !1, this.node = t;
  }
  update() {
  }
}
class Dd extends Ce {
  /**
   * We dynamically generate the AnimationState manager as it contains a reference
   * to the underlying animation library. We only want to load that if we load this,
   * so people can optionally code split it out using the `m` component.
   */
  constructor(t) {
    super(t), t.animationState || (t.animationState = Pd(t));
  }
  updateAnimationControlsSubscription() {
    const { animate: t } = this.node.getProps();
    en(t) && (this.unmountControls = t.subscribe(this.node));
  }
  /**
   * Subscribe any provided AnimationControls to the component's VisualElement
   */
  mount() {
    this.updateAnimationControlsSubscription();
  }
  update() {
    const { animate: t } = this.node.getProps(), { animate: n } = this.node.prevProps || {};
    t !== n && this.updateAnimationControlsSubscription();
  }
  unmount() {
    var t;
    this.node.animationState.reset(), (t = this.unmountControls) === null || t === void 0 || t.call(this);
  }
}
let jd = 0;
class kd extends Ce {
  constructor() {
    super(...arguments), this.id = jd++;
  }
  update() {
    if (!this.node.presenceContext)
      return;
    const { isPresent: t, onExitComplete: n } = this.node.presenceContext, { isPresent: s } = this.node.prevPresenceContext || {};
    if (!this.node.animationState || t === s)
      return;
    const r = this.node.animationState.setActive("exit", !t);
    n && !t && r.then(() => n(this.id));
  }
  mount() {
    const { register: t } = this.node.presenceContext || {};
    t && (this.unmount = t(this.id));
  }
  unmount() {
  }
}
const Nd = {
  animation: {
    Feature: Dd
  },
  exit: {
    Feature: kd
  }
};
function Tt(e, t, n, s = { passive: !0 }) {
  return e.addEventListener(t, n, s), () => e.removeEventListener(t, n);
}
function wt(e) {
  return {
    point: {
      x: e.pageX,
      y: e.pageY
    }
  };
}
const Vd = (e) => (t) => ys(t) && e(t, wt(t));
function ht(e, t, n, s) {
  return Tt(e, t, Vd(n), s);
}
const Pr = (e, t) => Math.abs(e - t);
function Md(e, t) {
  const n = Pr(e.x, t.x), s = Pr(e.y, t.y);
  return Math.sqrt(n ** 2 + s ** 2);
}
class Wo {
  constructor(t, n, { transformPagePoint: s, contextWindow: r, dragSnapToOrigin: o = !1 } = {}) {
    if (this.startEvent = null, this.lastMoveEvent = null, this.lastMoveEventInfo = null, this.handlers = {}, this.contextWindow = window, this.updatePoint = () => {
      if (!(this.lastMoveEvent && this.lastMoveEventInfo))
        return;
      const d = Sn(this.lastMoveEventInfo, this.history), f = this.startEvent !== null, m = Md(d.offset, { x: 0, y: 0 }) >= 3;
      if (!f && !m)
        return;
      const { point: g } = d, { timestamp: v } = G;
      this.history.push({ ...g, timestamp: v });
      const { onStart: T, onMove: x } = this.handlers;
      f || (T && T(this.lastMoveEvent, d), this.startEvent = this.lastMoveEvent), x && x(this.lastMoveEvent, d);
    }, this.handlePointerMove = (d, f) => {
      this.lastMoveEvent = d, this.lastMoveEventInfo = En(f, this.transformPagePoint), _.update(this.updatePoint, !0);
    }, this.handlePointerUp = (d, f) => {
      this.end();
      const { onEnd: m, onSessionEnd: g, resumeAnimation: v } = this.handlers;
      if (this.dragSnapToOrigin && v && v(), !(this.lastMoveEvent && this.lastMoveEventInfo))
        return;
      const T = Sn(d.type === "pointercancel" ? this.lastMoveEventInfo : En(f, this.transformPagePoint), this.history);
      this.startEvent && m && m(d, T), g && g(d, T);
    }, !ys(t))
      return;
    this.dragSnapToOrigin = o, this.handlers = n, this.transformPagePoint = s, this.contextWindow = r || window;
    const i = wt(t), a = En(i, this.transformPagePoint), { point: l } = a, { timestamp: c } = G;
    this.history = [{ ...l, timestamp: c }];
    const { onSessionStart: u } = n;
    u && u(t, Sn(a, this.history)), this.removeListeners = St(ht(this.contextWindow, "pointermove", this.handlePointerMove), ht(this.contextWindow, "pointerup", this.handlePointerUp), ht(this.contextWindow, "pointercancel", this.handlePointerUp));
  }
  updateHandlers(t) {
    this.handlers = t;
  }
  end() {
    this.removeListeners && this.removeListeners(), Re(this.updatePoint);
  }
}
function En(e, t) {
  return t ? { point: t(e.point) } : e;
}
function Cr(e, t) {
  return { x: e.x - t.x, y: e.y - t.y };
}
function Sn({ point: e }, t) {
  return {
    point: e,
    delta: Cr(e, Ho(t)),
    offset: Cr(e, Od(t)),
    velocity: Ld(t, 0.1)
  };
}
function Od(e) {
  return e[0];
}
function Ho(e) {
  return e[e.length - 1];
}
function Ld(e, t) {
  if (e.length < 2)
    return { x: 0, y: 0 };
  let n = e.length - 1, s = null;
  const r = Ho(e);
  for (; n >= 0 && (s = e[n], !(r.timestamp - s.timestamp > /* @__PURE__ */ me(t))); )
    n--;
  if (!s)
    return { x: 0, y: 0 };
  const o = /* @__PURE__ */ xe(r.timestamp - s.timestamp);
  if (o === 0)
    return { x: 0, y: 0 };
  const i = {
    x: (r.x - s.x) / o,
    y: (r.y - s.y) / o
  };
  return i.x === 1 / 0 && (i.x = 0), i.y === 1 / 0 && (i.y = 0), i;
}
const Go = 1e-4, Id = 1 - Go, _d = 1 + Go, zo = 0.01, Fd = 0 - zo, Bd = 0 + zo;
function re(e) {
  return e.max - e.min;
}
function Ud(e, t, n) {
  return Math.abs(e - t) <= n;
}
function Dr(e, t, n, s = 0.5) {
  e.origin = s, e.originPoint = B(t.min, t.max, e.origin), e.scale = re(n) / re(t), e.translate = B(n.min, n.max, e.origin) - e.originPoint, (e.scale >= Id && e.scale <= _d || isNaN(e.scale)) && (e.scale = 1), (e.translate >= Fd && e.translate <= Bd || isNaN(e.translate)) && (e.translate = 0);
}
function ft(e, t, n, s) {
  Dr(e.x, t.x, n.x, s ? s.originX : void 0), Dr(e.y, t.y, n.y, s ? s.originY : void 0);
}
function jr(e, t, n) {
  e.min = n.min + t.min, e.max = e.min + re(t);
}
function $d(e, t, n) {
  jr(e.x, t.x, n.x), jr(e.y, t.y, n.y);
}
function kr(e, t, n) {
  e.min = t.min - n.min, e.max = e.min + re(t);
}
function pt(e, t, n) {
  kr(e.x, t.x, n.x), kr(e.y, t.y, n.y);
}
function Kd(e, { min: t, max: n }, s) {
  return t !== void 0 && e < t ? e = s ? B(t, e, s.min) : Math.max(e, t) : n !== void 0 && e > n && (e = s ? B(n, e, s.max) : Math.min(e, n)), e;
}
function Nr(e, t, n) {
  return {
    min: t !== void 0 ? e.min + t : void 0,
    max: n !== void 0 ? e.max + n - (e.max - e.min) : void 0
  };
}
function Wd(e, { top: t, left: n, bottom: s, right: r }) {
  return {
    x: Nr(e.x, n, r),
    y: Nr(e.y, t, s)
  };
}
function Vr(e, t) {
  let n = t.min - e.min, s = t.max - e.max;
  return t.max - t.min < e.max - e.min && ([n, s] = [s, n]), { min: n, max: s };
}
function Hd(e, t) {
  return {
    x: Vr(e.x, t.x),
    y: Vr(e.y, t.y)
  };
}
function Gd(e, t) {
  let n = 0.5;
  const s = re(e), r = re(t);
  return r > s ? n = /* @__PURE__ */ Xe(t.min, t.max - s, e.min) : s > r && (n = /* @__PURE__ */ Xe(e.min, e.max - r, t.min)), be(0, 1, n);
}
function zd(e, t) {
  const n = {};
  return t.min !== void 0 && (n.min = t.min - e.min), t.max !== void 0 && (n.max = t.max - e.min), n;
}
const Bn = 0.35;
function Yd(e = Bn) {
  return e === !1 ? e = 0 : e === !0 && (e = Bn), {
    x: Mr(e, "left", "right"),
    y: Mr(e, "top", "bottom")
  };
}
function Mr(e, t, n) {
  return {
    min: Or(e, t),
    max: Or(e, n)
  };
}
function Or(e, t) {
  return typeof e == "number" ? e : e[t] || 0;
}
const Lr = () => ({
  translate: 0,
  scale: 1,
  origin: 0,
  originPoint: 0
}), He = () => ({
  x: Lr(),
  y: Lr()
}), Ir = () => ({ min: 0, max: 0 }), K = () => ({
  x: Ir(),
  y: Ir()
});
function le(e) {
  return [e("x"), e("y")];
}
function Yo({ top: e, left: t, right: n, bottom: s }) {
  return {
    x: { min: t, max: n },
    y: { min: e, max: s }
  };
}
function Xd({ x: e, y: t }) {
  return { top: t.min, right: e.max, bottom: t.max, left: e.min };
}
function qd(e, t) {
  if (!t)
    return e;
  const n = t({ x: e.left, y: e.top }), s = t({ x: e.right, y: e.bottom });
  return {
    top: n.y,
    left: n.x,
    bottom: s.y,
    right: s.x
  };
}
function wn(e) {
  return e === void 0 || e === 1;
}
function Un({ scale: e, scaleX: t, scaleY: n }) {
  return !wn(e) || !wn(t) || !wn(n);
}
function ke(e) {
  return Un(e) || Xo(e) || e.z || e.rotate || e.rotateX || e.rotateY || e.skewX || e.skewY;
}
function Xo(e) {
  return _r(e.x) || _r(e.y);
}
function _r(e) {
  return e && e !== "0%";
}
function zt(e, t, n) {
  const s = e - n, r = t * s;
  return n + r;
}
function Fr(e, t, n, s, r) {
  return r !== void 0 && (e = zt(e, r, s)), zt(e, n, s) + t;
}
function $n(e, t = 0, n = 1, s, r) {
  e.min = Fr(e.min, t, n, s, r), e.max = Fr(e.max, t, n, s, r);
}
function qo(e, { x: t, y: n }) {
  $n(e.x, t.translate, t.scale, t.originPoint), $n(e.y, n.translate, n.scale, n.originPoint);
}
const Br = 0.999999999999, Ur = 1.0000000000001;
function Zd(e, t, n, s = !1) {
  const r = n.length;
  if (!r)
    return;
  t.x = t.y = 1;
  let o, i;
  for (let a = 0; a < r; a++) {
    o = n[a], i = o.projectionDelta;
    const { visualElement: l } = o.options;
    l && l.props.style && l.props.style.display === "contents" || (s && o.options.layoutScroll && o.scroll && o !== o.root && ze(e, {
      x: -o.scroll.offset.x,
      y: -o.scroll.offset.y
    }), i && (t.x *= i.x.scale, t.y *= i.y.scale, qo(e, i)), s && ke(o.latestValues) && ze(e, o.latestValues));
  }
  t.x < Ur && t.x > Br && (t.x = 1), t.y < Ur && t.y > Br && (t.y = 1);
}
function Ge(e, t) {
  e.min = e.min + t, e.max = e.max + t;
}
function $r(e, t, n, s, r = 0.5) {
  const o = B(e.min, e.max, r);
  $n(e, t, n, o, s);
}
function ze(e, t) {
  $r(e.x, t.x, t.scaleX, t.scale, t.originX), $r(e.y, t.y, t.scaleY, t.scale, t.originY);
}
function Zo(e, t) {
  return Yo(qd(e.getBoundingClientRect(), t));
}
function Jd(e, t, n) {
  const s = Zo(e, n), { scroll: r } = t;
  return r && (Ge(s.x, r.offset.x), Ge(s.y, r.offset.y)), s;
}
const Jo = ({ current: e }) => e ? e.ownerDocument.defaultView : null, Qd = /* @__PURE__ */ new WeakMap();
class eh {
  constructor(t) {
    this.openDragLock = null, this.isDragging = !1, this.currentDirection = null, this.originPoint = { x: 0, y: 0 }, this.constraints = !1, this.hasMutatedConstraints = !1, this.elastic = K(), this.visualElement = t;
  }
  start(t, { snapToCursor: n = !1 } = {}) {
    const { presenceContext: s } = this.visualElement;
    if (s && s.isPresent === !1)
      return;
    const r = (u) => {
      const { dragSnapToOrigin: d } = this.getProps();
      d ? this.pauseAnimation() : this.stopAnimation(), n && this.snapToCursor(wt(u).point);
    }, o = (u, d) => {
      const { drag: f, dragPropagation: m, onDragStart: g } = this.getProps();
      if (f && !m && (this.openDragLock && this.openDragLock(), this.openDragLock = Gc(f), !this.openDragLock))
        return;
      this.isDragging = !0, this.currentDirection = null, this.resolveConstraints(), this.visualElement.projection && (this.visualElement.projection.isAnimationBlocked = !0, this.visualElement.projection.target = void 0), le((T) => {
        let x = this.getAxisMotionValue(T).get() || 0;
        if (ge.test(x)) {
          const { projection: b } = this.visualElement;
          if (b && b.layout) {
            const w = b.layout.layoutBox[T];
            w && (x = re(w) * (parseFloat(x) / 100));
          }
        }
        this.originPoint[T] = x;
      }), g && _.postRender(() => g(u, d)), Nn(this.visualElement, "transform");
      const { animationState: v } = this.visualElement;
      v && v.setActive("whileDrag", !0);
    }, i = (u, d) => {
      const { dragPropagation: f, dragDirectionLock: m, onDirectionLock: g, onDrag: v } = this.getProps();
      if (!f && !this.openDragLock)
        return;
      const { offset: T } = d;
      if (m && this.currentDirection === null) {
        this.currentDirection = th(T), this.currentDirection !== null && g && g(this.currentDirection);
        return;
      }
      this.updateAxis("x", d.point, T), this.updateAxis("y", d.point, T), this.visualElement.render(), v && v(u, d);
    }, a = (u, d) => this.stop(u, d), l = () => le((u) => {
      var d;
      return this.getAnimationState(u) === "paused" && ((d = this.getAxisMotionValue(u).animation) === null || d === void 0 ? void 0 : d.play());
    }), { dragSnapToOrigin: c } = this.getProps();
    this.panSession = new Wo(t, {
      onSessionStart: r,
      onStart: o,
      onMove: i,
      onSessionEnd: a,
      resumeAnimation: l
    }, {
      transformPagePoint: this.visualElement.getTransformPagePoint(),
      dragSnapToOrigin: c,
      contextWindow: Jo(this.visualElement)
    });
  }
  stop(t, n) {
    const s = this.isDragging;
    if (this.cancel(), !s)
      return;
    const { velocity: r } = n;
    this.startAnimation(r);
    const { onDragEnd: o } = this.getProps();
    o && _.postRender(() => o(t, n));
  }
  cancel() {
    this.isDragging = !1;
    const { projection: t, animationState: n } = this.visualElement;
    t && (t.isAnimationBlocked = !1), this.panSession && this.panSession.end(), this.panSession = void 0;
    const { dragPropagation: s } = this.getProps();
    !s && this.openDragLock && (this.openDragLock(), this.openDragLock = null), n && n.setActive("whileDrag", !1);
  }
  updateAxis(t, n, s) {
    const { drag: r } = this.getProps();
    if (!s || !Vt(t, r, this.currentDirection))
      return;
    const o = this.getAxisMotionValue(t);
    let i = this.originPoint[t] + s[t];
    this.constraints && this.constraints[t] && (i = Kd(i, this.constraints[t], this.elastic[t])), o.set(i);
  }
  resolveConstraints() {
    var t;
    const { dragConstraints: n, dragElastic: s } = this.getProps(), r = this.visualElement.projection && !this.visualElement.projection.layout ? this.visualElement.projection.measure(!1) : (t = this.visualElement.projection) === null || t === void 0 ? void 0 : t.layout, o = this.constraints;
    n && Ke(n) ? this.constraints || (this.constraints = this.resolveRefConstraints()) : n && r ? this.constraints = Wd(r.layoutBox, n) : this.constraints = !1, this.elastic = Yd(s), o !== this.constraints && r && this.constraints && !this.hasMutatedConstraints && le((i) => {
      this.constraints !== !1 && this.getAxisMotionValue(i) && (this.constraints[i] = zd(r.layoutBox[i], this.constraints[i]));
    });
  }
  resolveRefConstraints() {
    const { dragConstraints: t, onMeasureDragConstraints: n } = this.getProps();
    if (!t || !Ke(t))
      return !1;
    const s = t.current;
    Ae(s !== null, "If `dragConstraints` is set as a React ref, that ref must be passed to another component's `ref` prop.");
    const { projection: r } = this.visualElement;
    if (!r || !r.layout)
      return !1;
    const o = Jd(s, r.root, this.visualElement.getTransformPagePoint());
    let i = Hd(r.layout.layoutBox, o);
    if (n) {
      const a = n(Xd(i));
      this.hasMutatedConstraints = !!a, a && (i = Yo(a));
    }
    return i;
  }
  startAnimation(t) {
    const { drag: n, dragMomentum: s, dragElastic: r, dragTransition: o, dragSnapToOrigin: i, onDragTransitionEnd: a } = this.getProps(), l = this.constraints || {}, c = le((u) => {
      if (!Vt(u, n, this.currentDirection))
        return;
      let d = l && l[u] || {};
      i && (d = { min: 0, max: 0 });
      const f = r ? 200 : 1e6, m = r ? 40 : 1e7, g = {
        type: "inertia",
        velocity: s ? t[u] : 0,
        bounceStiffness: f,
        bounceDamping: m,
        timeConstant: 750,
        restDelta: 1,
        restSpeed: 10,
        ...o,
        ...d
      };
      return this.startAxisValueAnimation(u, g);
    });
    return Promise.all(c).then(a);
  }
  startAxisValueAnimation(t, n) {
    const s = this.getAxisMotionValue(t);
    return Nn(this.visualElement, t), s.start(Ds(t, s, 0, n, this.visualElement, !1));
  }
  stopAnimation() {
    le((t) => this.getAxisMotionValue(t).stop());
  }
  pauseAnimation() {
    le((t) => {
      var n;
      return (n = this.getAxisMotionValue(t).animation) === null || n === void 0 ? void 0 : n.pause();
    });
  }
  getAnimationState(t) {
    var n;
    return (n = this.getAxisMotionValue(t).animation) === null || n === void 0 ? void 0 : n.state;
  }
  /**
   * Drag works differently depending on which props are provided.
   *
   * - If _dragX and _dragY are provided, we output the gesture delta directly to those motion values.
   * - Otherwise, we apply the delta to the x/y motion values.
   */
  getAxisMotionValue(t) {
    const n = `_drag${t.toUpperCase()}`, s = this.visualElement.getProps(), r = s[n];
    return r || this.visualElement.getValue(t, (s.initial ? s.initial[t] : void 0) || 0);
  }
  snapToCursor(t) {
    le((n) => {
      const { drag: s } = this.getProps();
      if (!Vt(n, s, this.currentDirection))
        return;
      const { projection: r } = this.visualElement, o = this.getAxisMotionValue(n);
      if (r && r.layout) {
        const { min: i, max: a } = r.layout.layoutBox[n];
        o.set(t[n] - B(i, a, 0.5));
      }
    });
  }
  /**
   * When the viewport resizes we want to check if the measured constraints
   * have changed and, if so, reposition the element within those new constraints
   * relative to where it was before the resize.
   */
  scalePositionWithinConstraints() {
    if (!this.visualElement.current)
      return;
    const { drag: t, dragConstraints: n } = this.getProps(), { projection: s } = this.visualElement;
    if (!Ke(n) || !s || !this.constraints)
      return;
    this.stopAnimation();
    const r = { x: 0, y: 0 };
    le((i) => {
      const a = this.getAxisMotionValue(i);
      if (a && this.constraints !== !1) {
        const l = a.get();
        r[i] = Gd({ min: l, max: l }, this.constraints[i]);
      }
    });
    const { transformTemplate: o } = this.visualElement.getProps();
    this.visualElement.current.style.transform = o ? o({}, "") : "none", s.root && s.root.updateScroll(), s.updateLayout(), this.resolveConstraints(), le((i) => {
      if (!Vt(i, t, null))
        return;
      const a = this.getAxisMotionValue(i), { min: l, max: c } = this.constraints[i];
      a.set(B(l, c, r[i]));
    });
  }
  addListeners() {
    if (!this.visualElement.current)
      return;
    Qd.set(this.visualElement, this);
    const t = this.visualElement.current, n = ht(t, "pointerdown", (l) => {
      const { drag: c, dragListener: u = !0 } = this.getProps();
      c && u && this.start(l);
    }), s = () => {
      const { dragConstraints: l } = this.getProps();
      Ke(l) && l.current && (this.constraints = this.resolveRefConstraints());
    }, { projection: r } = this.visualElement, o = r.addEventListener("measure", s);
    r && !r.layout && (r.root && r.root.updateScroll(), r.updateLayout()), _.read(s);
    const i = Tt(window, "resize", () => this.scalePositionWithinConstraints()), a = r.addEventListener("didUpdate", (({ delta: l, hasLayoutChanged: c }) => {
      this.isDragging && c && (le((u) => {
        const d = this.getAxisMotionValue(u);
        d && (this.originPoint[u] += l[u].translate, d.set(d.get() + l[u].translate));
      }), this.visualElement.render());
    }));
    return () => {
      i(), n(), o(), a && a();
    };
  }
  getProps() {
    const t = this.visualElement.getProps(), { drag: n = !1, dragDirectionLock: s = !1, dragPropagation: r = !1, dragConstraints: o = !1, dragElastic: i = Bn, dragMomentum: a = !0 } = t;
    return {
      ...t,
      drag: n,
      dragDirectionLock: s,
      dragPropagation: r,
      dragConstraints: o,
      dragElastic: i,
      dragMomentum: a
    };
  }
}
function Vt(e, t, n) {
  return (t === !0 || t === e) && (n === null || n === e);
}
function th(e, t = 10) {
  let n = null;
  return Math.abs(e.y) > t ? n = "y" : Math.abs(e.x) > t && (n = "x"), n;
}
class nh extends Ce {
  constructor(t) {
    super(t), this.removeGroupControls = ee, this.removeListeners = ee, this.controls = new eh(t);
  }
  mount() {
    const { dragControls: t } = this.node.getProps();
    t && (this.removeGroupControls = t.subscribe(this.controls)), this.removeListeners = this.controls.addListeners() || ee;
  }
  unmount() {
    this.removeGroupControls(), this.removeListeners();
  }
}
const Kr = (e) => (t, n) => {
  e && _.postRender(() => e(t, n));
};
class sh extends Ce {
  constructor() {
    super(...arguments), this.removePointerDownListener = ee;
  }
  onPointerDown(t) {
    this.session = new Wo(t, this.createPanHandlers(), {
      transformPagePoint: this.node.getTransformPagePoint(),
      contextWindow: Jo(this.node)
    });
  }
  createPanHandlers() {
    const { onPanSessionStart: t, onPanStart: n, onPan: s, onPanEnd: r } = this.node.getProps();
    return {
      onSessionStart: Kr(t),
      onStart: Kr(n),
      onMove: s,
      onEnd: (o, i) => {
        delete this.session, r && _.postRender(() => r(o, i));
      }
    };
  }
  mount() {
    this.removePointerDownListener = ht(this.node.current, "pointerdown", (t) => this.onPointerDown(t));
  }
  update() {
    this.session && this.session.updateHandlers(this.createPanHandlers());
  }
  unmount() {
    this.removePointerDownListener(), this.session && this.session.end();
  }
}
const _t = {
  /**
   * Global flag as to whether the tree has animated since the last time
   * we resized the window
   */
  hasAnimatedSinceResize: !0,
  /**
   * We set this to true once, on the first update. Any nodes added to the tree beyond that
   * update will be given a `data-projection-id` attribute.
   */
  hasEverUpdated: !1
};
function Wr(e, t) {
  return t.max === t.min ? 0 : e / (t.max - t.min) * 100;
}
const it = {
  correct: (e, t) => {
    if (!t.target)
      return e;
    if (typeof e == "string")
      if (A.test(e))
        e = parseFloat(e);
      else
        return e;
    const n = Wr(e, t.target.x), s = Wr(e, t.target.y);
    return `${n}% ${s}%`;
  }
}, rh = {
  correct: (e, { treeScale: t, projectionDelta: n }) => {
    const s = e, r = Pe.parse(e);
    if (r.length > 5)
      return s;
    const o = Pe.createTransformer(e), i = typeof r[0] != "number" ? 1 : 0, a = n.x.scale * t.x, l = n.y.scale * t.y;
    r[0 + i] /= a, r[1 + i] /= l;
    const c = B(a, l, 0.5);
    return typeof r[2 + i] == "number" && (r[2 + i] /= c), typeof r[3 + i] == "number" && (r[3 + i] /= c), o(r);
  }
};
class ih extends Ga {
  /**
   * This only mounts projection nodes for components that
   * need measuring, we might want to do it for all components
   * in order to incorporate transforms
   */
  componentDidMount() {
    const { visualElement: t, layoutGroup: n, switchLayoutGroup: s, layoutId: r } = this.props, { projection: o } = t;
    Ac(oh), o && (n.group && n.group.add(o), s && s.register && r && s.register(o), o.root.didUpdate(), o.addEventListener("animationComplete", () => {
      this.safeToRemove();
    }), o.setOptions({
      ...o.options,
      onExitComplete: () => this.safeToRemove()
    })), _t.hasEverUpdated = !0;
  }
  getSnapshotBeforeUpdate(t) {
    const { layoutDependency: n, visualElement: s, drag: r, isPresent: o } = this.props, i = s.projection;
    return i && (i.isPresent = o, r || t.layoutDependency !== n || n === void 0 ? i.willUpdate() : this.safeToRemove(), t.isPresent !== o && (o ? i.promote() : i.relegate() || _.postRender(() => {
      const a = i.getStack();
      (!a || !a.members.length) && this.safeToRemove();
    }))), null;
  }
  componentDidUpdate() {
    const { projection: t } = this.props.visualElement;
    t && (t.root.didUpdate(), rs.postRender(() => {
      !t.currentAnimation && t.isLead() && this.safeToRemove();
    }));
  }
  componentWillUnmount() {
    const { visualElement: t, layoutGroup: n, switchLayoutGroup: s } = this.props, { projection: r } = t;
    r && (r.scheduleCheckAfterUnmount(), n && n.group && n.group.remove(r), s && s.deregister && s.deregister(r));
  }
  safeToRemove() {
    const { safeToRemove: t } = this.props;
    t && t();
  }
  render() {
    return null;
  }
}
function Qo(e) {
  const [t, n] = Oi(), s = X(qn);
  return p.jsx(ih, { ...e, layoutGroup: s, switchLayoutGroup: X($i), isPresent: t, safeToRemove: n });
}
const oh = {
  borderRadius: {
    ...it,
    applyTo: [
      "borderTopLeftRadius",
      "borderTopRightRadius",
      "borderBottomLeftRadius",
      "borderBottomRightRadius"
    ]
  },
  borderTopLeftRadius: it,
  borderTopRightRadius: it,
  borderBottomLeftRadius: it,
  borderBottomRightRadius: it,
  boxShadow: rh
};
function ah(e, t, n) {
  const s = Z(e) ? e : vt(e);
  return s.start(Ds("", s, t, n)), s.animation;
}
function lh(e) {
  return e instanceof SVGElement && e.tagName !== "svg";
}
const ch = (e, t) => e.depth - t.depth;
class uh {
  constructor() {
    this.children = [], this.isDirty = !1;
  }
  add(t) {
    vs(this.children, t), this.isDirty = !0;
  }
  remove(t) {
    xs(this.children, t), this.isDirty = !0;
  }
  forEach(t) {
    this.isDirty && this.children.sort(ch), this.isDirty = !1, this.children.forEach(t);
  }
}
function dh(e, t) {
  const n = ye.now(), s = ({ timestamp: r }) => {
    const o = r - n;
    o >= t && (Re(s), e(o - t));
  };
  return _.read(s, !0), () => Re(s);
}
const ea = ["TopLeft", "TopRight", "BottomLeft", "BottomRight"], hh = ea.length, Hr = (e) => typeof e == "string" ? parseFloat(e) : e, Gr = (e) => typeof e == "number" || A.test(e);
function fh(e, t, n, s, r, o) {
  r ? (e.opacity = B(
    0,
    // TODO Reinstate this if only child
    n.opacity !== void 0 ? n.opacity : 1,
    ph(s)
  ), e.opacityExit = B(t.opacity !== void 0 ? t.opacity : 1, 0, mh(s))) : o && (e.opacity = B(t.opacity !== void 0 ? t.opacity : 1, n.opacity !== void 0 ? n.opacity : 1, s));
  for (let i = 0; i < hh; i++) {
    const a = `border${ea[i]}Radius`;
    let l = zr(t, a), c = zr(n, a);
    if (l === void 0 && c === void 0)
      continue;
    l || (l = 0), c || (c = 0), l === 0 || c === 0 || Gr(l) === Gr(c) ? (e[a] = Math.max(B(Hr(l), Hr(c), s), 0), (ge.test(c) || ge.test(l)) && (e[a] += "%")) : e[a] = c;
  }
  (t.rotate || n.rotate) && (e.rotate = B(t.rotate || 0, n.rotate || 0, s));
}
function zr(e, t) {
  return e[t] !== void 0 ? e[t] : e.borderRadius;
}
const ph = /* @__PURE__ */ ta(0, 0.5, xo), mh = /* @__PURE__ */ ta(0.5, 0.95, ee);
function ta(e, t, n) {
  return (s) => s < e ? 0 : s > t ? 1 : n(/* @__PURE__ */ Xe(e, t, s));
}
function Yr(e, t) {
  e.min = t.min, e.max = t.max;
}
function ae(e, t) {
  Yr(e.x, t.x), Yr(e.y, t.y);
}
function Xr(e, t) {
  e.translate = t.translate, e.scale = t.scale, e.originPoint = t.originPoint, e.origin = t.origin;
}
function qr(e, t, n, s, r) {
  return e -= t, e = zt(e, 1 / n, s), r !== void 0 && (e = zt(e, 1 / r, s)), e;
}
function gh(e, t = 0, n = 1, s = 0.5, r, o = e, i = e) {
  if (ge.test(t) && (t = parseFloat(t), t = B(i.min, i.max, t / 100) - i.min), typeof t != "number")
    return;
  let a = B(o.min, o.max, s);
  e === o && (a -= t), e.min = qr(e.min, t, n, a, r), e.max = qr(e.max, t, n, a, r);
}
function Zr(e, t, [n, s, r], o, i) {
  gh(e, t[n], t[s], t[r], t.scale, o, i);
}
const yh = ["x", "scaleX", "originX"], vh = ["y", "scaleY", "originY"];
function Jr(e, t, n, s) {
  Zr(e.x, t, yh, n ? n.x : void 0, s ? s.x : void 0), Zr(e.y, t, vh, n ? n.y : void 0, s ? s.y : void 0);
}
function Qr(e) {
  return e.translate === 0 && e.scale === 1;
}
function na(e) {
  return Qr(e.x) && Qr(e.y);
}
function ei(e, t) {
  return e.min === t.min && e.max === t.max;
}
function xh(e, t) {
  return ei(e.x, t.x) && ei(e.y, t.y);
}
function ti(e, t) {
  return Math.round(e.min) === Math.round(t.min) && Math.round(e.max) === Math.round(t.max);
}
function sa(e, t) {
  return ti(e.x, t.x) && ti(e.y, t.y);
}
function ni(e) {
  return re(e.x) / re(e.y);
}
function si(e, t) {
  return e.translate === t.translate && e.scale === t.scale && e.originPoint === t.originPoint;
}
class Th {
  constructor() {
    this.members = [];
  }
  add(t) {
    vs(this.members, t), t.scheduleRender();
  }
  remove(t) {
    if (xs(this.members, t), t === this.prevLead && (this.prevLead = void 0), t === this.lead) {
      const n = this.members[this.members.length - 1];
      n && this.promote(n);
    }
  }
  relegate(t) {
    const n = this.members.findIndex((r) => t === r);
    if (n === 0)
      return !1;
    let s;
    for (let r = n; r >= 0; r--) {
      const o = this.members[r];
      if (o.isPresent !== !1) {
        s = o;
        break;
      }
    }
    return s ? (this.promote(s), !0) : !1;
  }
  promote(t, n) {
    const s = this.lead;
    if (t !== s && (this.prevLead = s, this.lead = t, t.show(), s)) {
      s.instance && s.scheduleRender(), t.scheduleRender(), t.resumeFrom = s, n && (t.resumeFrom.preserveOpacity = !0), s.snapshot && (t.snapshot = s.snapshot, t.snapshot.latestValues = s.animationValues || s.latestValues), t.root && t.root.isUpdating && (t.isLayoutDirty = !0);
      const { crossfade: r } = t.options;
      r === !1 && s.hide();
    }
  }
  exitAnimationComplete() {
    this.members.forEach((t) => {
      const { options: n, resumingFrom: s } = t;
      n.onExitComplete && n.onExitComplete(), s && s.options.onExitComplete && s.options.onExitComplete();
    });
  }
  scheduleRender() {
    this.members.forEach((t) => {
      t.instance && t.scheduleRender(!1);
    });
  }
  /**
   * Clear any leads that have been removed this render to prevent them from being
   * used in future animations and to prevent memory leaks
   */
  removeLeadSnapshot() {
    this.lead && this.lead.snapshot && (this.lead.snapshot = void 0);
  }
}
function bh(e, t, n) {
  let s = "";
  const r = e.x.translate / t.x, o = e.y.translate / t.y, i = (n == null ? void 0 : n.z) || 0;
  if ((r || o || i) && (s = `translate3d(${r}px, ${o}px, ${i}px) `), (t.x !== 1 || t.y !== 1) && (s += `scale(${1 / t.x}, ${1 / t.y}) `), n) {
    const { transformPerspective: c, rotate: u, rotateX: d, rotateY: f, skewX: m, skewY: g } = n;
    c && (s = `perspective(${c}px) ${s}`), u && (s += `rotate(${u}deg) `), d && (s += `rotateX(${d}deg) `), f && (s += `rotateY(${f}deg) `), m && (s += `skewX(${m}deg) `), g && (s += `skewY(${g}deg) `);
  }
  const a = e.x.scale * t.x, l = e.y.scale * t.y;
  return (a !== 1 || l !== 1) && (s += `scale(${a}, ${l})`), s || "none";
}
const Ne = {
  type: "projectionFrame",
  totalNodes: 0,
  resolvedTargetDeltas: 0,
  recalculatedProjection: 0
}, ct = typeof window < "u" && window.MotionDebug !== void 0, An = ["", "X", "Y", "Z"], Eh = { visibility: "hidden" }, ri = 1e3;
let Sh = 0;
function Rn(e, t, n, s) {
  const { latestValues: r } = t;
  r[e] && (n[e] = r[e], t.setStaticValue(e, 0), s && (s[e] = 0));
}
function ra(e) {
  if (e.hasCheckedOptimisedAppear = !0, e.root === e)
    return;
  const { visualElement: t } = e.options;
  if (!t)
    return;
  const n = ho(t);
  if (window.MotionHasOptimisedAnimation(n, "transform")) {
    const { layout: r, layoutId: o } = e.options;
    window.MotionCancelOptimisedAnimation(n, "transform", _, !(r || o));
  }
  const { parent: s } = e;
  s && !s.hasCheckedOptimisedAppear && ra(s);
}
function ia({ attachResizeListener: e, defaultParent: t, measureScroll: n, checkIsScrollRoot: s, resetTransform: r }) {
  return class {
    constructor(i = {}, a = t == null ? void 0 : t()) {
      this.id = Sh++, this.animationId = 0, this.children = /* @__PURE__ */ new Set(), this.options = {}, this.isTreeAnimating = !1, this.isAnimationBlocked = !1, this.isLayoutDirty = !1, this.isProjectionDirty = !1, this.isSharedProjectionDirty = !1, this.isTransformDirty = !1, this.updateManuallyBlocked = !1, this.updateBlockedByResize = !1, this.isUpdating = !1, this.isSVG = !1, this.needsReset = !1, this.shouldResetTransform = !1, this.hasCheckedOptimisedAppear = !1, this.treeScale = { x: 1, y: 1 }, this.eventHandlers = /* @__PURE__ */ new Map(), this.hasTreeAnimated = !1, this.updateScheduled = !1, this.scheduleUpdate = () => this.update(), this.projectionUpdateScheduled = !1, this.checkUpdateFailed = () => {
        this.isUpdating && (this.isUpdating = !1, this.clearAllSnapshots());
      }, this.updateProjection = () => {
        this.projectionUpdateScheduled = !1, ct && (Ne.totalNodes = Ne.resolvedTargetDeltas = Ne.recalculatedProjection = 0), this.nodes.forEach(Rh), this.nodes.forEach(kh), this.nodes.forEach(Nh), this.nodes.forEach(Ph), ct && window.MotionDebug.record(Ne);
      }, this.resolvedRelativeTargetAt = 0, this.hasProjected = !1, this.isVisible = !0, this.animationProgress = 0, this.sharedNodes = /* @__PURE__ */ new Map(), this.latestValues = i, this.root = a ? a.root || a : this, this.path = a ? [...a.path, a] : [], this.parent = a, this.depth = a ? a.depth + 1 : 0;
      for (let l = 0; l < this.path.length; l++)
        this.path[l].shouldResetTransform = !0;
      this.root === this && (this.nodes = new uh());
    }
    addEventListener(i, a) {
      return this.eventHandlers.has(i) || this.eventHandlers.set(i, new Ts()), this.eventHandlers.get(i).add(a);
    }
    notifyListeners(i, ...a) {
      const l = this.eventHandlers.get(i);
      l && l.notify(...a);
    }
    hasListeners(i) {
      return this.eventHandlers.has(i);
    }
    /**
     * Lifecycles
     */
    mount(i, a = this.root.hasTreeAnimated) {
      if (this.instance)
        return;
      this.isSVG = lh(i), this.instance = i;
      const { layoutId: l, layout: c, visualElement: u } = this.options;
      if (u && !u.current && u.mount(i), this.root.nodes.add(this), this.parent && this.parent.children.add(this), a && (c || l) && (this.isLayoutDirty = !0), e) {
        let d;
        const f = () => this.root.updateBlockedByResize = !1;
        e(i, () => {
          this.root.updateBlockedByResize = !0, d && d(), d = dh(f, 250), _t.hasAnimatedSinceResize && (_t.hasAnimatedSinceResize = !1, this.nodes.forEach(oi));
        });
      }
      l && this.root.registerSharedNode(l, this), this.options.animate !== !1 && u && (l || c) && this.addEventListener("didUpdate", ({ delta: d, hasLayoutChanged: f, hasRelativeTargetChanged: m, layout: g }) => {
        if (this.isTreeAnimationBlocked()) {
          this.target = void 0, this.relativeTarget = void 0;
          return;
        }
        const v = this.options.transition || u.getDefaultTransition() || Ih, { onLayoutAnimationStart: T, onLayoutAnimationComplete: x } = u.getProps(), b = !this.targetLayout || !sa(this.targetLayout, g) || m, w = !f && m;
        if (this.options.layoutRoot || this.resumeFrom && this.resumeFrom.instance || w || f && (b || !this.currentAnimation)) {
          this.resumeFrom && (this.resumingFrom = this.resumeFrom, this.resumingFrom.resumingFrom = void 0), this.setAnimationOrigin(d, w);
          const P = {
            ...ps(v, "layout"),
            onPlay: T,
            onComplete: x
          };
          (u.shouldReduceMotion || this.options.layoutRoot) && (P.delay = 0, P.type = !1), this.startAnimation(P);
        } else
          f || oi(this), this.isLead() && this.options.onExitComplete && this.options.onExitComplete();
        this.targetLayout = g;
      });
    }
    unmount() {
      this.options.layoutId && this.willUpdate(), this.root.nodes.remove(this);
      const i = this.getStack();
      i && i.remove(this), this.parent && this.parent.children.delete(this), this.instance = void 0, Re(this.updateProjection);
    }
    // only on the root
    blockUpdate() {
      this.updateManuallyBlocked = !0;
    }
    unblockUpdate() {
      this.updateManuallyBlocked = !1;
    }
    isUpdateBlocked() {
      return this.updateManuallyBlocked || this.updateBlockedByResize;
    }
    isTreeAnimationBlocked() {
      return this.isAnimationBlocked || this.parent && this.parent.isTreeAnimationBlocked() || !1;
    }
    // Note: currently only running on root node
    startUpdate() {
      this.isUpdateBlocked() || (this.isUpdating = !0, this.nodes && this.nodes.forEach(Vh), this.animationId++);
    }
    getTransformTemplate() {
      const { visualElement: i } = this.options;
      return i && i.getProps().transformTemplate;
    }
    willUpdate(i = !0) {
      if (this.root.hasTreeAnimated = !0, this.root.isUpdateBlocked()) {
        this.options.onExitComplete && this.options.onExitComplete();
        return;
      }
      if (window.MotionCancelOptimisedAnimation && !this.hasCheckedOptimisedAppear && ra(this), !this.root.isUpdating && this.root.startUpdate(), this.isLayoutDirty)
        return;
      this.isLayoutDirty = !0;
      for (let u = 0; u < this.path.length; u++) {
        const d = this.path[u];
        d.shouldResetTransform = !0, d.updateScroll("snapshot"), d.options.layoutRoot && d.willUpdate(!1);
      }
      const { layoutId: a, layout: l } = this.options;
      if (a === void 0 && !l)
        return;
      const c = this.getTransformTemplate();
      this.prevTransformTemplateValue = c ? c(this.latestValues, "") : void 0, this.updateSnapshot(), i && this.notifyListeners("willUpdate");
    }
    update() {
      if (this.updateScheduled = !1, this.isUpdateBlocked()) {
        this.unblockUpdate(), this.clearAllSnapshots(), this.nodes.forEach(ii);
        return;
      }
      this.isUpdating || this.nodes.forEach(Dh), this.isUpdating = !1, this.nodes.forEach(jh), this.nodes.forEach(wh), this.nodes.forEach(Ah), this.clearAllSnapshots();
      const a = ye.now();
      G.delta = be(0, 1e3 / 60, a - G.timestamp), G.timestamp = a, G.isProcessing = !0, gn.update.process(G), gn.preRender.process(G), gn.render.process(G), G.isProcessing = !1;
    }
    didUpdate() {
      this.updateScheduled || (this.updateScheduled = !0, rs.read(this.scheduleUpdate));
    }
    clearAllSnapshots() {
      this.nodes.forEach(Ch), this.sharedNodes.forEach(Mh);
    }
    scheduleUpdateProjection() {
      this.projectionUpdateScheduled || (this.projectionUpdateScheduled = !0, _.preRender(this.updateProjection, !1, !0));
    }
    scheduleCheckAfterUnmount() {
      _.postRender(() => {
        this.isLayoutDirty ? this.root.didUpdate() : this.root.checkUpdateFailed();
      });
    }
    /**
     * Update measurements
     */
    updateSnapshot() {
      this.snapshot || !this.instance || (this.snapshot = this.measure());
    }
    updateLayout() {
      if (!this.instance || (this.updateScroll(), !(this.options.alwaysMeasureLayout && this.isLead()) && !this.isLayoutDirty))
        return;
      if (this.resumeFrom && !this.resumeFrom.instance)
        for (let l = 0; l < this.path.length; l++)
          this.path[l].updateScroll();
      const i = this.layout;
      this.layout = this.measure(!1), this.layoutCorrected = K(), this.isLayoutDirty = !1, this.projectionDelta = void 0, this.notifyListeners("measure", this.layout.layoutBox);
      const { visualElement: a } = this.options;
      a && a.notify("LayoutMeasure", this.layout.layoutBox, i ? i.layoutBox : void 0);
    }
    updateScroll(i = "measure") {
      let a = !!(this.options.layoutScroll && this.instance);
      if (this.scroll && this.scroll.animationId === this.root.animationId && this.scroll.phase === i && (a = !1), a) {
        const l = s(this.instance);
        this.scroll = {
          animationId: this.root.animationId,
          phase: i,
          isRoot: l,
          offset: n(this.instance),
          wasRoot: this.scroll ? this.scroll.isRoot : l
        };
      }
    }
    resetTransform() {
      if (!r)
        return;
      const i = this.isLayoutDirty || this.shouldResetTransform || this.options.alwaysMeasureLayout, a = this.projectionDelta && !na(this.projectionDelta), l = this.getTransformTemplate(), c = l ? l(this.latestValues, "") : void 0, u = c !== this.prevTransformTemplateValue;
      i && (a || ke(this.latestValues) || u) && (r(this.instance, c), this.shouldResetTransform = !1, this.scheduleRender());
    }
    measure(i = !0) {
      const a = this.measurePageBox();
      let l = this.removeElementScroll(a);
      return i && (l = this.removeTransform(l)), _h(l), {
        animationId: this.root.animationId,
        measuredBox: a,
        layoutBox: l,
        latestValues: {},
        source: this.id
      };
    }
    measurePageBox() {
      var i;
      const { visualElement: a } = this.options;
      if (!a)
        return K();
      const l = a.measureViewportBox();
      if (!(((i = this.scroll) === null || i === void 0 ? void 0 : i.wasRoot) || this.path.some(Fh))) {
        const { scroll: u } = this.root;
        u && (Ge(l.x, u.offset.x), Ge(l.y, u.offset.y));
      }
      return l;
    }
    removeElementScroll(i) {
      var a;
      const l = K();
      if (ae(l, i), !((a = this.scroll) === null || a === void 0) && a.wasRoot)
        return l;
      for (let c = 0; c < this.path.length; c++) {
        const u = this.path[c], { scroll: d, options: f } = u;
        u !== this.root && d && f.layoutScroll && (d.wasRoot && ae(l, i), Ge(l.x, d.offset.x), Ge(l.y, d.offset.y));
      }
      return l;
    }
    applyTransform(i, a = !1) {
      const l = K();
      ae(l, i);
      for (let c = 0; c < this.path.length; c++) {
        const u = this.path[c];
        !a && u.options.layoutScroll && u.scroll && u !== u.root && ze(l, {
          x: -u.scroll.offset.x,
          y: -u.scroll.offset.y
        }), ke(u.latestValues) && ze(l, u.latestValues);
      }
      return ke(this.latestValues) && ze(l, this.latestValues), l;
    }
    removeTransform(i) {
      const a = K();
      ae(a, i);
      for (let l = 0; l < this.path.length; l++) {
        const c = this.path[l];
        if (!c.instance || !ke(c.latestValues))
          continue;
        Un(c.latestValues) && c.updateSnapshot();
        const u = K(), d = c.measurePageBox();
        ae(u, d), Jr(a, c.latestValues, c.snapshot ? c.snapshot.layoutBox : void 0, u);
      }
      return ke(this.latestValues) && Jr(a, this.latestValues), a;
    }
    setTargetDelta(i) {
      this.targetDelta = i, this.root.scheduleUpdateProjection(), this.isProjectionDirty = !0;
    }
    setOptions(i) {
      this.options = {
        ...this.options,
        ...i,
        crossfade: i.crossfade !== void 0 ? i.crossfade : !0
      };
    }
    clearMeasurements() {
      this.scroll = void 0, this.layout = void 0, this.snapshot = void 0, this.prevTransformTemplateValue = void 0, this.targetDelta = void 0, this.target = void 0, this.isLayoutDirty = !1;
    }
    forceRelativeParentToResolveTarget() {
      this.relativeParent && this.relativeParent.resolvedRelativeTargetAt !== G.timestamp && this.relativeParent.resolveTargetDelta(!0);
    }
    resolveTargetDelta(i = !1) {
      var a;
      const l = this.getLead();
      this.isProjectionDirty || (this.isProjectionDirty = l.isProjectionDirty), this.isTransformDirty || (this.isTransformDirty = l.isTransformDirty), this.isSharedProjectionDirty || (this.isSharedProjectionDirty = l.isSharedProjectionDirty);
      const c = !!this.resumingFrom || this !== l;
      if (!(i || c && this.isSharedProjectionDirty || this.isProjectionDirty || !((a = this.parent) === null || a === void 0) && a.isProjectionDirty || this.attemptToResolveRelativeTarget || this.root.updateBlockedByResize))
        return;
      const { layout: d, layoutId: f } = this.options;
      if (!(!this.layout || !(d || f))) {
        if (this.resolvedRelativeTargetAt = G.timestamp, !this.targetDelta && !this.relativeTarget) {
          const m = this.getClosestProjectingParent();
          m && m.layout && this.animationProgress !== 1 ? (this.relativeParent = m, this.forceRelativeParentToResolveTarget(), this.relativeTarget = K(), this.relativeTargetOrigin = K(), pt(this.relativeTargetOrigin, this.layout.layoutBox, m.layout.layoutBox), ae(this.relativeTarget, this.relativeTargetOrigin)) : this.relativeParent = this.relativeTarget = void 0;
        }
        if (!(!this.relativeTarget && !this.targetDelta)) {
          if (this.target || (this.target = K(), this.targetWithTransforms = K()), this.relativeTarget && this.relativeTargetOrigin && this.relativeParent && this.relativeParent.target ? (this.forceRelativeParentToResolveTarget(), $d(this.target, this.relativeTarget, this.relativeParent.target)) : this.targetDelta ? (this.resumingFrom ? this.target = this.applyTransform(this.layout.layoutBox) : ae(this.target, this.layout.layoutBox), qo(this.target, this.targetDelta)) : ae(this.target, this.layout.layoutBox), this.attemptToResolveRelativeTarget) {
            this.attemptToResolveRelativeTarget = !1;
            const m = this.getClosestProjectingParent();
            m && !!m.resumingFrom == !!this.resumingFrom && !m.options.layoutScroll && m.target && this.animationProgress !== 1 ? (this.relativeParent = m, this.forceRelativeParentToResolveTarget(), this.relativeTarget = K(), this.relativeTargetOrigin = K(), pt(this.relativeTargetOrigin, this.target, m.target), ae(this.relativeTarget, this.relativeTargetOrigin)) : this.relativeParent = this.relativeTarget = void 0;
          }
          ct && Ne.resolvedTargetDeltas++;
        }
      }
    }
    getClosestProjectingParent() {
      if (!(!this.parent || Un(this.parent.latestValues) || Xo(this.parent.latestValues)))
        return this.parent.isProjecting() ? this.parent : this.parent.getClosestProjectingParent();
    }
    isProjecting() {
      return !!((this.relativeTarget || this.targetDelta || this.options.layoutRoot) && this.layout);
    }
    calcProjection() {
      var i;
      const a = this.getLead(), l = !!this.resumingFrom || this !== a;
      let c = !0;
      if ((this.isProjectionDirty || !((i = this.parent) === null || i === void 0) && i.isProjectionDirty) && (c = !1), l && (this.isSharedProjectionDirty || this.isTransformDirty) && (c = !1), this.resolvedRelativeTargetAt === G.timestamp && (c = !1), c)
        return;
      const { layout: u, layoutId: d } = this.options;
      if (this.isTreeAnimating = !!(this.parent && this.parent.isTreeAnimating || this.currentAnimation || this.pendingAnimation), this.isTreeAnimating || (this.targetDelta = this.relativeTarget = void 0), !this.layout || !(u || d))
        return;
      ae(this.layoutCorrected, this.layout.layoutBox);
      const f = this.treeScale.x, m = this.treeScale.y;
      Zd(this.layoutCorrected, this.treeScale, this.path, l), a.layout && !a.target && (this.treeScale.x !== 1 || this.treeScale.y !== 1) && (a.target = a.layout.layoutBox, a.targetWithTransforms = K());
      const { target: g } = a;
      if (!g) {
        this.prevProjectionDelta && (this.createProjectionDeltas(), this.scheduleRender());
        return;
      }
      !this.projectionDelta || !this.prevProjectionDelta ? this.createProjectionDeltas() : (Xr(this.prevProjectionDelta.x, this.projectionDelta.x), Xr(this.prevProjectionDelta.y, this.projectionDelta.y)), ft(this.projectionDelta, this.layoutCorrected, g, this.latestValues), (this.treeScale.x !== f || this.treeScale.y !== m || !si(this.projectionDelta.x, this.prevProjectionDelta.x) || !si(this.projectionDelta.y, this.prevProjectionDelta.y)) && (this.hasProjected = !0, this.scheduleRender(), this.notifyListeners("projectionUpdate", g)), ct && Ne.recalculatedProjection++;
    }
    hide() {
      this.isVisible = !1;
    }
    show() {
      this.isVisible = !0;
    }
    scheduleRender(i = !0) {
      var a;
      if ((a = this.options.visualElement) === null || a === void 0 || a.scheduleRender(), i) {
        const l = this.getStack();
        l && l.scheduleRender();
      }
      this.resumingFrom && !this.resumingFrom.instance && (this.resumingFrom = void 0);
    }
    createProjectionDeltas() {
      this.prevProjectionDelta = He(), this.projectionDelta = He(), this.projectionDeltaWithTransform = He();
    }
    setAnimationOrigin(i, a = !1) {
      const l = this.snapshot, c = l ? l.latestValues : {}, u = { ...this.latestValues }, d = He();
      (!this.relativeParent || !this.relativeParent.options.layoutRoot) && (this.relativeTarget = this.relativeTargetOrigin = void 0), this.attemptToResolveRelativeTarget = !a;
      const f = K(), m = l ? l.source : void 0, g = this.layout ? this.layout.source : void 0, v = m !== g, T = this.getStack(), x = !T || T.members.length <= 1, b = !!(v && !x && this.options.crossfade === !0 && !this.path.some(Lh));
      this.animationProgress = 0;
      let w;
      this.mixTargetDelta = (P) => {
        const S = P / 1e3;
        ai(d.x, i.x, S), ai(d.y, i.y, S), this.setTargetDelta(d), this.relativeTarget && this.relativeTargetOrigin && this.layout && this.relativeParent && this.relativeParent.layout && (pt(f, this.layout.layoutBox, this.relativeParent.layout.layoutBox), Oh(this.relativeTarget, this.relativeTargetOrigin, f, S), w && xh(this.relativeTarget, w) && (this.isProjectionDirty = !1), w || (w = K()), ae(w, this.relativeTarget)), v && (this.animationValues = u, fh(u, c, this.latestValues, S, b, x)), this.root.scheduleUpdateProjection(), this.scheduleRender(), this.animationProgress = S;
      }, this.mixTargetDelta(this.options.layoutRoot ? 1e3 : 0);
    }
    startAnimation(i) {
      this.notifyListeners("animationStart"), this.currentAnimation && this.currentAnimation.stop(), this.resumingFrom && this.resumingFrom.currentAnimation && this.resumingFrom.currentAnimation.stop(), this.pendingAnimation && (Re(this.pendingAnimation), this.pendingAnimation = void 0), this.pendingAnimation = _.update(() => {
        _t.hasAnimatedSinceResize = !0, this.currentAnimation = ah(0, ri, {
          ...i,
          onUpdate: (a) => {
            this.mixTargetDelta(a), i.onUpdate && i.onUpdate(a);
          },
          onComplete: () => {
            i.onComplete && i.onComplete(), this.completeAnimation();
          }
        }), this.resumingFrom && (this.resumingFrom.currentAnimation = this.currentAnimation), this.pendingAnimation = void 0;
      });
    }
    completeAnimation() {
      this.resumingFrom && (this.resumingFrom.currentAnimation = void 0, this.resumingFrom.preserveOpacity = void 0);
      const i = this.getStack();
      i && i.exitAnimationComplete(), this.resumingFrom = this.currentAnimation = this.animationValues = void 0, this.notifyListeners("animationComplete");
    }
    finishAnimation() {
      this.currentAnimation && (this.mixTargetDelta && this.mixTargetDelta(ri), this.currentAnimation.stop()), this.completeAnimation();
    }
    applyTransformsToTarget() {
      const i = this.getLead();
      let { targetWithTransforms: a, target: l, layout: c, latestValues: u } = i;
      if (!(!a || !l || !c)) {
        if (this !== i && this.layout && c && oa(this.options.animationType, this.layout.layoutBox, c.layoutBox)) {
          l = this.target || K();
          const d = re(this.layout.layoutBox.x);
          l.x.min = i.target.x.min, l.x.max = l.x.min + d;
          const f = re(this.layout.layoutBox.y);
          l.y.min = i.target.y.min, l.y.max = l.y.min + f;
        }
        ae(a, l), ze(a, u), ft(this.projectionDeltaWithTransform, this.layoutCorrected, a, u);
      }
    }
    registerSharedNode(i, a) {
      this.sharedNodes.has(i) || this.sharedNodes.set(i, new Th()), this.sharedNodes.get(i).add(a);
      const c = a.options.initialPromotionConfig;
      a.promote({
        transition: c ? c.transition : void 0,
        preserveFollowOpacity: c && c.shouldPreserveFollowOpacity ? c.shouldPreserveFollowOpacity(a) : void 0
      });
    }
    isLead() {
      const i = this.getStack();
      return i ? i.lead === this : !0;
    }
    getLead() {
      var i;
      const { layoutId: a } = this.options;
      return a ? ((i = this.getStack()) === null || i === void 0 ? void 0 : i.lead) || this : this;
    }
    getPrevLead() {
      var i;
      const { layoutId: a } = this.options;
      return a ? (i = this.getStack()) === null || i === void 0 ? void 0 : i.prevLead : void 0;
    }
    getStack() {
      const { layoutId: i } = this.options;
      if (i)
        return this.root.sharedNodes.get(i);
    }
    promote({ needsReset: i, transition: a, preserveFollowOpacity: l } = {}) {
      const c = this.getStack();
      c && c.promote(this, l), i && (this.projectionDelta = void 0, this.needsReset = !0), a && this.setOptions({ transition: a });
    }
    relegate() {
      const i = this.getStack();
      return i ? i.relegate(this) : !1;
    }
    resetSkewAndRotation() {
      const { visualElement: i } = this.options;
      if (!i)
        return;
      let a = !1;
      const { latestValues: l } = i;
      if ((l.z || l.rotate || l.rotateX || l.rotateY || l.rotateZ || l.skewX || l.skewY) && (a = !0), !a)
        return;
      const c = {};
      l.z && Rn("z", i, c, this.animationValues);
      for (let u = 0; u < An.length; u++)
        Rn(`rotate${An[u]}`, i, c, this.animationValues), Rn(`skew${An[u]}`, i, c, this.animationValues);
      i.render();
      for (const u in c)
        i.setStaticValue(u, c[u]), this.animationValues && (this.animationValues[u] = c[u]);
      i.scheduleRender();
    }
    getProjectionStyles(i) {
      var a, l;
      if (!this.instance || this.isSVG)
        return;
      if (!this.isVisible)
        return Eh;
      const c = {
        visibility: ""
      }, u = this.getTransformTemplate();
      if (this.needsReset)
        return this.needsReset = !1, c.opacity = "", c.pointerEvents = Lt(i == null ? void 0 : i.pointerEvents) || "", c.transform = u ? u(this.latestValues, "") : "none", c;
      const d = this.getLead();
      if (!this.projectionDelta || !this.layout || !d.target) {
        const v = {};
        return this.options.layoutId && (v.opacity = this.latestValues.opacity !== void 0 ? this.latestValues.opacity : 1, v.pointerEvents = Lt(i == null ? void 0 : i.pointerEvents) || ""), this.hasProjected && !ke(this.latestValues) && (v.transform = u ? u({}, "") : "none", this.hasProjected = !1), v;
      }
      const f = d.animationValues || d.latestValues;
      this.applyTransformsToTarget(), c.transform = bh(this.projectionDeltaWithTransform, this.treeScale, f), u && (c.transform = u(f, c.transform));
      const { x: m, y: g } = this.projectionDelta;
      c.transformOrigin = `${m.origin * 100}% ${g.origin * 100}% 0`, d.animationValues ? c.opacity = d === this ? (l = (a = f.opacity) !== null && a !== void 0 ? a : this.latestValues.opacity) !== null && l !== void 0 ? l : 1 : this.preserveOpacity ? this.latestValues.opacity : f.opacityExit : c.opacity = d === this ? f.opacity !== void 0 ? f.opacity : "" : f.opacityExit !== void 0 ? f.opacityExit : 0;
      for (const v in $t) {
        if (f[v] === void 0)
          continue;
        const { correct: T, applyTo: x } = $t[v], b = c.transform === "none" ? f[v] : T(f[v], d);
        if (x) {
          const w = x.length;
          for (let P = 0; P < w; P++)
            c[x[P]] = b;
        } else
          c[v] = b;
      }
      return this.options.layoutId && (c.pointerEvents = d === this ? Lt(i == null ? void 0 : i.pointerEvents) || "" : "none"), c;
    }
    clearSnapshot() {
      this.resumeFrom = this.snapshot = void 0;
    }
    // Only run on root
    resetTree() {
      this.root.nodes.forEach((i) => {
        var a;
        return (a = i.currentAnimation) === null || a === void 0 ? void 0 : a.stop();
      }), this.root.nodes.forEach(ii), this.root.sharedNodes.clear();
    }
  };
}
function wh(e) {
  e.updateLayout();
}
function Ah(e) {
  var t;
  const n = ((t = e.resumeFrom) === null || t === void 0 ? void 0 : t.snapshot) || e.snapshot;
  if (e.isLead() && e.layout && n && e.hasListeners("didUpdate")) {
    const { layoutBox: s, measuredBox: r } = e.layout, { animationType: o } = e.options, i = n.source !== e.layout.source;
    o === "size" ? le((d) => {
      const f = i ? n.measuredBox[d] : n.layoutBox[d], m = re(f);
      f.min = s[d].min, f.max = f.min + m;
    }) : oa(o, n.layoutBox, s) && le((d) => {
      const f = i ? n.measuredBox[d] : n.layoutBox[d], m = re(s[d]);
      f.max = f.min + m, e.relativeTarget && !e.currentAnimation && (e.isProjectionDirty = !0, e.relativeTarget[d].max = e.relativeTarget[d].min + m);
    });
    const a = He();
    ft(a, s, n.layoutBox);
    const l = He();
    i ? ft(l, e.applyTransform(r, !0), n.measuredBox) : ft(l, s, n.layoutBox);
    const c = !na(a);
    let u = !1;
    if (!e.resumeFrom) {
      const d = e.getClosestProjectingParent();
      if (d && !d.resumeFrom) {
        const { snapshot: f, layout: m } = d;
        if (f && m) {
          const g = K();
          pt(g, n.layoutBox, f.layoutBox);
          const v = K();
          pt(v, s, m.layoutBox), sa(g, v) || (u = !0), d.options.layoutRoot && (e.relativeTarget = v, e.relativeTargetOrigin = g, e.relativeParent = d);
        }
      }
    }
    e.notifyListeners("didUpdate", {
      layout: s,
      snapshot: n,
      delta: l,
      layoutDelta: a,
      hasLayoutChanged: c,
      hasRelativeTargetChanged: u
    });
  } else if (e.isLead()) {
    const { onExitComplete: s } = e.options;
    s && s();
  }
  e.options.transition = void 0;
}
function Rh(e) {
  ct && Ne.totalNodes++, e.parent && (e.isProjecting() || (e.isProjectionDirty = e.parent.isProjectionDirty), e.isSharedProjectionDirty || (e.isSharedProjectionDirty = !!(e.isProjectionDirty || e.parent.isProjectionDirty || e.parent.isSharedProjectionDirty)), e.isTransformDirty || (e.isTransformDirty = e.parent.isTransformDirty));
}
function Ph(e) {
  e.isProjectionDirty = e.isSharedProjectionDirty = e.isTransformDirty = !1;
}
function Ch(e) {
  e.clearSnapshot();
}
function ii(e) {
  e.clearMeasurements();
}
function Dh(e) {
  e.isLayoutDirty = !1;
}
function jh(e) {
  const { visualElement: t } = e.options;
  t && t.getProps().onBeforeLayoutMeasure && t.notify("BeforeLayoutMeasure"), e.resetTransform();
}
function oi(e) {
  e.finishAnimation(), e.targetDelta = e.relativeTarget = e.target = void 0, e.isProjectionDirty = !0;
}
function kh(e) {
  e.resolveTargetDelta();
}
function Nh(e) {
  e.calcProjection();
}
function Vh(e) {
  e.resetSkewAndRotation();
}
function Mh(e) {
  e.removeLeadSnapshot();
}
function ai(e, t, n) {
  e.translate = B(t.translate, 0, n), e.scale = B(t.scale, 1, n), e.origin = t.origin, e.originPoint = t.originPoint;
}
function li(e, t, n, s) {
  e.min = B(t.min, n.min, s), e.max = B(t.max, n.max, s);
}
function Oh(e, t, n, s) {
  li(e.x, t.x, n.x, s), li(e.y, t.y, n.y, s);
}
function Lh(e) {
  return e.animationValues && e.animationValues.opacityExit !== void 0;
}
const Ih = {
  duration: 0.45,
  ease: [0.4, 0, 0.1, 1]
}, ci = (e) => typeof navigator < "u" && navigator.userAgent && navigator.userAgent.toLowerCase().includes(e), ui = ci("applewebkit/") && !ci("chrome/") ? Math.round : ee;
function di(e) {
  e.min = ui(e.min), e.max = ui(e.max);
}
function _h(e) {
  di(e.x), di(e.y);
}
function oa(e, t, n) {
  return e === "position" || e === "preserve-aspect" && !Ud(ni(t), ni(n), 0.2);
}
function Fh(e) {
  var t;
  return e !== e.root && ((t = e.scroll) === null || t === void 0 ? void 0 : t.wasRoot);
}
const Bh = ia({
  attachResizeListener: (e, t) => Tt(e, "resize", t),
  measureScroll: () => ({
    x: document.documentElement.scrollLeft || document.body.scrollLeft,
    y: document.documentElement.scrollTop || document.body.scrollTop
  }),
  checkIsScrollRoot: () => !0
}), Pn = {
  current: void 0
}, aa = ia({
  measureScroll: (e) => ({
    x: e.scrollLeft,
    y: e.scrollTop
  }),
  defaultParent: () => {
    if (!Pn.current) {
      const e = new Bh({});
      e.mount(window), e.setOptions({ layoutScroll: !0 }), Pn.current = e;
    }
    return Pn.current;
  },
  resetTransform: (e, t) => {
    e.style.transform = t !== void 0 ? t : "none";
  },
  checkIsScrollRoot: (e) => window.getComputedStyle(e).position === "fixed"
}), Uh = {
  pan: {
    Feature: sh
  },
  drag: {
    Feature: nh,
    ProjectionNode: aa,
    MeasureLayout: Qo
  }
};
function hi(e, t, n) {
  const { props: s } = e;
  e.animationState && s.whileHover && e.animationState.setActive("whileHover", n === "Start");
  const r = "onHover" + n, o = s[r];
  o && _.postRender(() => o(t, wt(t)));
}
class $h extends Ce {
  mount() {
    const { current: t } = this.node;
    t && (this.unmount = Uc(t, (n) => (hi(this.node, n, "Start"), (s) => hi(this.node, s, "End"))));
  }
  unmount() {
  }
}
class Kh extends Ce {
  constructor() {
    super(...arguments), this.isActive = !1;
  }
  onFocus() {
    let t = !1;
    try {
      t = this.node.current.matches(":focus-visible");
    } catch {
      t = !0;
    }
    !t || !this.node.animationState || (this.node.animationState.setActive("whileFocus", !0), this.isActive = !0);
  }
  onBlur() {
    !this.isActive || !this.node.animationState || (this.node.animationState.setActive("whileFocus", !1), this.isActive = !1);
  }
  mount() {
    this.unmount = St(Tt(this.node.current, "focus", () => this.onFocus()), Tt(this.node.current, "blur", () => this.onBlur()));
  }
  unmount() {
  }
}
function fi(e, t, n) {
  const { props: s } = e;
  e.animationState && s.whileTap && e.animationState.setActive("whileTap", n === "Start");
  const r = "onTap" + (n === "End" ? "" : n), o = s[r];
  o && _.postRender(() => o(t, wt(t)));
}
class Wh extends Ce {
  mount() {
    const { current: t } = this.node;
    t && (this.unmount = Hc(t, (n) => (fi(this.node, n, "Start"), (s, { success: r }) => fi(this.node, s, r ? "End" : "Cancel")), { useGlobalTarget: this.node.props.globalTapTarget }));
  }
  unmount() {
  }
}
const Kn = /* @__PURE__ */ new WeakMap(), Cn = /* @__PURE__ */ new WeakMap(), Hh = (e) => {
  const t = Kn.get(e.target);
  t && t(e);
}, Gh = (e) => {
  e.forEach(Hh);
};
function zh({ root: e, ...t }) {
  const n = e || document;
  Cn.has(n) || Cn.set(n, {});
  const s = Cn.get(n), r = JSON.stringify(t);
  return s[r] || (s[r] = new IntersectionObserver(Gh, { root: e, ...t })), s[r];
}
function Yh(e, t, n) {
  const s = zh(t);
  return Kn.set(e, n), s.observe(e), () => {
    Kn.delete(e), s.unobserve(e);
  };
}
const Xh = {
  some: 0,
  all: 1
};
class qh extends Ce {
  constructor() {
    super(...arguments), this.hasEnteredView = !1, this.isInView = !1;
  }
  startObserver() {
    this.unmount();
    const { viewport: t = {} } = this.node.getProps(), { root: n, margin: s, amount: r = "some", once: o } = t, i = {
      root: n ? n.current : void 0,
      rootMargin: s,
      threshold: typeof r == "number" ? r : Xh[r]
    }, a = (l) => {
      const { isIntersecting: c } = l;
      if (this.isInView === c || (this.isInView = c, o && !c && this.hasEnteredView))
        return;
      c && (this.hasEnteredView = !0), this.node.animationState && this.node.animationState.setActive("whileInView", c);
      const { onViewportEnter: u, onViewportLeave: d } = this.node.getProps(), f = c ? u : d;
      f && f(l);
    };
    return Yh(this.node.current, i, a);
  }
  mount() {
    this.startObserver();
  }
  update() {
    if (typeof IntersectionObserver > "u")
      return;
    const { props: t, prevProps: n } = this.node;
    ["amount", "margin", "root"].some(Zh(t, n)) && this.startObserver();
  }
  unmount() {
  }
}
function Zh({ viewport: e = {} }, { viewport: t = {} } = {}) {
  return (n) => e[n] !== t[n];
}
const Jh = {
  inView: {
    Feature: qh
  },
  tap: {
    Feature: Wh
  },
  focus: {
    Feature: Kh
  },
  hover: {
    Feature: $h
  }
}, Qh = {
  layout: {
    ProjectionNode: aa,
    MeasureLayout: Qo
  }
}, Wn = { current: null }, la = { current: !1 };
function ef() {
  if (la.current = !0, !!Qn)
    if (window.matchMedia) {
      const e = window.matchMedia("(prefers-reduced-motion)"), t = () => Wn.current = e.matches;
      e.addListener(t), t();
    } else
      Wn.current = !1;
}
const tf = [...Vo, q, Pe], nf = (e) => tf.find(No(e)), pi = /* @__PURE__ */ new WeakMap();
function sf(e, t, n) {
  for (const s in t) {
    const r = t[s], o = n[s];
    if (Z(r))
      e.addValue(s, r), process.env.NODE_ENV === "development" && Jt(r.version === "11.18.2", `Attempting to mix Motion versions ${r.version} with 11.18.2 may not work as expected.`);
    else if (Z(o))
      e.addValue(s, vt(r, { owner: e }));
    else if (o !== r)
      if (e.hasValue(s)) {
        const i = e.getValue(s);
        i.liveStyle === !0 ? i.jump(r) : i.hasAnimated || i.set(r);
      } else {
        const i = e.getStaticValue(s);
        e.addValue(s, vt(i !== void 0 ? i : r, { owner: e }));
      }
  }
  for (const s in n)
    t[s] === void 0 && e.removeValue(s);
  return t;
}
const mi = [
  "AnimationStart",
  "AnimationComplete",
  "Update",
  "BeforeLayoutMeasure",
  "LayoutMeasure",
  "LayoutAnimationStart",
  "LayoutAnimationComplete"
];
class rf {
  /**
   * This method takes React props and returns found MotionValues. For example, HTML
   * MotionValues will be found within the style prop, whereas for Three.js within attribute arrays.
   *
   * This isn't an abstract method as it needs calling in the constructor, but it is
   * intended to be one.
   */
  scrapeMotionValuesFromProps(t, n, s) {
    return {};
  }
  constructor({ parent: t, props: n, presenceContext: s, reducedMotionConfig: r, blockInitialAnimation: o, visualState: i }, a = {}) {
    this.current = null, this.children = /* @__PURE__ */ new Set(), this.isVariantNode = !1, this.isControllingVariants = !1, this.shouldReduceMotion = null, this.values = /* @__PURE__ */ new Map(), this.KeyframeResolver = Rs, this.features = {}, this.valueSubscriptions = /* @__PURE__ */ new Map(), this.prevMotionValues = {}, this.events = {}, this.propEventSubscriptions = {}, this.notifyUpdate = () => this.notify("Update", this.latestValues), this.render = () => {
      this.current && (this.triggerBuild(), this.renderInstance(this.current, this.renderState, this.props.style, this.projection));
    }, this.renderScheduledAt = 0, this.scheduleRender = () => {
      const m = ye.now();
      this.renderScheduledAt < m && (this.renderScheduledAt = m, _.render(this.render, !1, !0));
    };
    const { latestValues: l, renderState: c, onUpdate: u } = i;
    this.onUpdate = u, this.latestValues = l, this.baseTarget = { ...l }, this.initialValues = n.initial ? { ...l } : {}, this.renderState = c, this.parent = t, this.props = n, this.presenceContext = s, this.depth = t ? t.depth + 1 : 0, this.reducedMotionConfig = r, this.options = a, this.blockInitialAnimation = !!o, this.isControllingVariants = tn(n), this.isVariantNode = Bi(n), this.isVariantNode && (this.variantChildren = /* @__PURE__ */ new Set()), this.manuallyAnimateOnMount = !!(t && t.current);
    const { willChange: d, ...f } = this.scrapeMotionValuesFromProps(n, {}, this);
    for (const m in f) {
      const g = f[m];
      l[m] !== void 0 && Z(g) && g.set(l[m], !1);
    }
  }
  mount(t) {
    this.current = t, pi.set(t, this), this.projection && !this.projection.instance && this.projection.mount(t), this.parent && this.isVariantNode && !this.isControllingVariants && (this.removeFromVariantTree = this.parent.addVariantChild(this)), this.values.forEach((n, s) => this.bindToMotionValue(s, n)), la.current || ef(), this.shouldReduceMotion = this.reducedMotionConfig === "never" ? !1 : this.reducedMotionConfig === "always" ? !0 : Wn.current, process.env.NODE_ENV !== "production" && Jt(this.shouldReduceMotion !== !0, "You have Reduced Motion enabled on your device. Animations may not appear as expected."), this.parent && this.parent.children.add(this), this.update(this.props, this.presenceContext);
  }
  unmount() {
    pi.delete(this.current), this.projection && this.projection.unmount(), Re(this.notifyUpdate), Re(this.render), this.valueSubscriptions.forEach((t) => t()), this.valueSubscriptions.clear(), this.removeFromVariantTree && this.removeFromVariantTree(), this.parent && this.parent.children.delete(this);
    for (const t in this.events)
      this.events[t].clear();
    for (const t in this.features) {
      const n = this.features[t];
      n && (n.unmount(), n.isMounted = !1);
    }
    this.current = null;
  }
  bindToMotionValue(t, n) {
    this.valueSubscriptions.has(t) && this.valueSubscriptions.get(t)();
    const s = Ie.has(t), r = n.on("change", (a) => {
      this.latestValues[t] = a, this.props.onUpdate && _.preRender(this.notifyUpdate), s && this.projection && (this.projection.isTransformDirty = !0);
    }), o = n.on("renderRequest", this.scheduleRender);
    let i;
    window.MotionCheckAppearSync && (i = window.MotionCheckAppearSync(this, t, n)), this.valueSubscriptions.set(t, () => {
      r(), o(), i && i(), n.owner && n.stop();
    });
  }
  sortNodePosition(t) {
    return !this.current || !this.sortInstanceNodePosition || this.type !== t.type ? 0 : this.sortInstanceNodePosition(this.current, t.current);
  }
  updateFeatures() {
    let t = "animation";
    for (t in qe) {
      const n = qe[t];
      if (!n)
        continue;
      const { isEnabled: s, Feature: r } = n;
      if (!this.features[t] && r && s(this.props) && (this.features[t] = new r(this)), this.features[t]) {
        const o = this.features[t];
        o.isMounted ? o.update() : (o.mount(), o.isMounted = !0);
      }
    }
  }
  triggerBuild() {
    this.build(this.renderState, this.latestValues, this.props);
  }
  /**
   * Measure the current viewport box with or without transforms.
   * Only measures axis-aligned boxes, rotate and skew must be manually
   * removed with a re-render to work.
   */
  measureViewportBox() {
    return this.current ? this.measureInstanceViewportBox(this.current, this.props) : K();
  }
  getStaticValue(t) {
    return this.latestValues[t];
  }
  setStaticValue(t, n) {
    this.latestValues[t] = n;
  }
  /**
   * Update the provided props. Ensure any newly-added motion values are
   * added to our map, old ones removed, and listeners updated.
   */
  update(t, n) {
    (t.transformTemplate || this.props.transformTemplate) && this.scheduleRender(), this.prevProps = this.props, this.props = t, this.prevPresenceContext = this.presenceContext, this.presenceContext = n;
    for (let s = 0; s < mi.length; s++) {
      const r = mi[s];
      this.propEventSubscriptions[r] && (this.propEventSubscriptions[r](), delete this.propEventSubscriptions[r]);
      const o = "on" + r, i = t[o];
      i && (this.propEventSubscriptions[r] = this.on(r, i));
    }
    this.prevMotionValues = sf(this, this.scrapeMotionValuesFromProps(t, this.prevProps, this), this.prevMotionValues), this.handleChildMotionValue && this.handleChildMotionValue(), this.onUpdate && this.onUpdate(this);
  }
  getProps() {
    return this.props;
  }
  /**
   * Returns the variant definition with a given name.
   */
  getVariant(t) {
    return this.props.variants ? this.props.variants[t] : void 0;
  }
  /**
   * Returns the defined default transition on this component.
   */
  getDefaultTransition() {
    return this.props.transition;
  }
  getTransformPagePoint() {
    return this.props.transformPagePoint;
  }
  getClosestVariantNode() {
    return this.isVariantNode ? this : this.parent ? this.parent.getClosestVariantNode() : void 0;
  }
  /**
   * Add a child visual element to our set of children.
   */
  addVariantChild(t) {
    const n = this.getClosestVariantNode();
    if (n)
      return n.variantChildren && n.variantChildren.add(t), () => n.variantChildren.delete(t);
  }
  /**
   * Add a motion value and bind it to this visual element.
   */
  addValue(t, n) {
    const s = this.values.get(t);
    n !== s && (s && this.removeValue(t), this.bindToMotionValue(t, n), this.values.set(t, n), this.latestValues[t] = n.get());
  }
  /**
   * Remove a motion value and unbind any active subscriptions.
   */
  removeValue(t) {
    this.values.delete(t);
    const n = this.valueSubscriptions.get(t);
    n && (n(), this.valueSubscriptions.delete(t)), delete this.latestValues[t], this.removeValueFromRenderState(t, this.renderState);
  }
  /**
   * Check whether we have a motion value for this key
   */
  hasValue(t) {
    return this.values.has(t);
  }
  getValue(t, n) {
    if (this.props.values && this.props.values[t])
      return this.props.values[t];
    let s = this.values.get(t);
    return s === void 0 && n !== void 0 && (s = vt(n === null ? void 0 : n, { owner: this }), this.addValue(t, s)), s;
  }
  /**
   * If we're trying to animate to a previously unencountered value,
   * we need to check for it in our state and as a last resort read it
   * directly from the instance (which might have performance implications).
   */
  readValue(t, n) {
    var s;
    let r = this.latestValues[t] !== void 0 || !this.current ? this.latestValues[t] : (s = this.getBaseTargetFromProps(this.props, t)) !== null && s !== void 0 ? s : this.readValueFromInstance(this.current, t, this.options);
    return r != null && (typeof r == "string" && (jo(r) || bo(r)) ? r = parseFloat(r) : !nf(r) && Pe.test(n) && (r = Po(t, n)), this.setBaseTarget(t, Z(r) ? r.get() : r)), Z(r) ? r.get() : r;
  }
  /**
   * Set the base target to later animate back to. This is currently
   * only hydrated on creation and when we first read a value.
   */
  setBaseTarget(t, n) {
    this.baseTarget[t] = n;
  }
  /**
   * Find the base target for a value thats been removed from all animation
   * props.
   */
  getBaseTarget(t) {
    var n;
    const { initial: s } = this.props;
    let r;
    if (typeof s == "string" || typeof s == "object") {
      const i = os(this.props, s, (n = this.presenceContext) === null || n === void 0 ? void 0 : n.custom);
      i && (r = i[t]);
    }
    if (s && r !== void 0)
      return r;
    const o = this.getBaseTargetFromProps(this.props, t);
    return o !== void 0 && !Z(o) ? o : this.initialValues[t] !== void 0 && r === void 0 ? void 0 : this.baseTarget[t];
  }
  on(t, n) {
    return this.events[t] || (this.events[t] = new Ts()), this.events[t].add(n);
  }
  notify(t, ...n) {
    this.events[t] && this.events[t].notify(...n);
  }
}
class ca extends rf {
  constructor() {
    super(...arguments), this.KeyframeResolver = Mo;
  }
  sortInstanceNodePosition(t, n) {
    return t.compareDocumentPosition(n) & 2 ? 1 : -1;
  }
  getBaseTargetFromProps(t, n) {
    return t.style ? t.style[n] : void 0;
  }
  removeValueFromRenderState(t, { vars: n, style: s }) {
    delete n[t], delete s[t];
  }
  handleChildMotionValue() {
    this.childSubscription && (this.childSubscription(), delete this.childSubscription);
    const { children: t } = this.props;
    Z(t) && (this.childSubscription = t.on("change", (n) => {
      this.current && (this.current.textContent = `${n}`);
    }));
  }
}
function of(e) {
  return window.getComputedStyle(e);
}
class af extends ca {
  constructor() {
    super(...arguments), this.type = "html", this.renderInstance = Xi;
  }
  readValueFromInstance(t, n) {
    if (Ie.has(n)) {
      const s = As(n);
      return s && s.default || 0;
    } else {
      const s = of(t), r = (Gi(n) ? s.getPropertyValue(n) : s[n]) || 0;
      return typeof r == "string" ? r.trim() : r;
    }
  }
  measureInstanceViewportBox(t, { transformPagePoint: n }) {
    return Zo(t, n);
  }
  build(t, n, s) {
    cs(t, n, s.transformTemplate);
  }
  scrapeMotionValuesFromProps(t, n, s) {
    return fs(t, n, s);
  }
}
class lf extends ca {
  constructor() {
    super(...arguments), this.type = "svg", this.isSVGTag = !1, this.measureInstanceViewportBox = K;
  }
  getBaseTargetFromProps(t, n) {
    return t[n];
  }
  readValueFromInstance(t, n) {
    if (Ie.has(n)) {
      const s = As(n);
      return s && s.default || 0;
    }
    return n = qi.has(n) ? n : ss(n), t.getAttribute(n);
  }
  scrapeMotionValuesFromProps(t, n, s) {
    return Qi(t, n, s);
  }
  build(t, n, s) {
    us(t, n, this.isSVGTag, s.transformTemplate);
  }
  renderInstance(t, n, s, r) {
    Zi(t, n, s, r);
  }
  mount(t) {
    this.isSVGTag = hs(t.tagName), super.mount(t);
  }
}
const cf = (e, t) => is(e) ? new lf(t) : new af(t, {
  allowProjection: e !== yi
}), uf = /* @__PURE__ */ Mc({
  ...Nd,
  ...Jh,
  ...Uh,
  ...Qh
}, cf), Te = /* @__PURE__ */ Xl(uf);
function ua(e) {
  var t, n, s = "";
  if (typeof e == "string" || typeof e == "number") s += e;
  else if (typeof e == "object") if (Array.isArray(e)) {
    var r = e.length;
    for (t = 0; t < r; t++) e[t] && (n = ua(e[t])) && (s && (s += " "), s += n);
  } else for (n in e) e[n] && (s && (s += " "), s += n);
  return s;
}
function se() {
  for (var e, t, n = 0, s = "", r = arguments.length; n < r; n++) (e = arguments[n]) && (t = ua(e)) && (s && (s += " "), s += t);
  return s;
}
function da({
  item: e,
  collapsed: t,
  depth: n = 0
}) {
  const s = Ti(), { hasPermission: r, hasRole: o } = ve(), [i, a] = Yt.useState(!1);
  if (e.permissions && !r(e.permissions) || e.roles && !o(e.roles) || e.isHidden) return null;
  if (e.isDivider)
    return /* @__PURE__ */ p.jsx("div", { className: "my-2 border-t border-slate-700/50" });
  const l = e.path ? s.pathname.startsWith(e.path) : !1, c = e.children && e.children.length > 0, u = () => {
    c && a(!i);
  }, d = /* @__PURE__ */ p.jsxs(p.Fragment, { children: [
    e.icon && /* @__PURE__ */ p.jsx("span", { className: se(
      "w-5 h-5 flex items-center justify-center text-lg",
      l ? "text-primary-400" : "text-slate-400 group-hover:text-primary-400"
    ), children: /* @__PURE__ */ p.jsx("i", { className: e.icon }) }),
    /* @__PURE__ */ p.jsx(Ye, { children: !t && /* @__PURE__ */ p.jsx(
      Te.span,
      {
        initial: { opacity: 0, width: 0 },
        animate: { opacity: 1, width: "auto" },
        exit: { opacity: 0, width: 0 },
        className: se(
          "flex-1 truncate text-sm font-medium transition-colors",
          l ? "text-white" : "text-slate-300 group-hover:text-white"
        ),
        children: e.label
      }
    ) }),
    e.badge && !t && /* @__PURE__ */ p.jsx("span", { className: se(
      "px-2 py-0.5 text-xs font-medium rounded-full",
      e.badgeColor === "danger" && "bg-red-500/20 text-red-400",
      e.badgeColor === "warning" && "bg-yellow-500/20 text-yellow-400",
      e.badgeColor === "success" && "bg-green-500/20 text-green-400",
      e.badgeColor === "info" && "bg-blue-500/20 text-blue-400",
      (!e.badgeColor || e.badgeColor === "primary") && "bg-primary-500/20 text-primary-400"
    ), children: e.badge }),
    c && !t && /* @__PURE__ */ p.jsx(
      Te.span,
      {
        animate: { rotate: i ? 90 : 0 },
        className: "text-slate-400",
        children: /* @__PURE__ */ p.jsx("svg", { className: "w-4 h-4", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M9 5l7 7-7 7" }) })
      }
    )
  ] }), f = se(
    "group flex items-center gap-3 px-3 py-2.5 rounded-lg transition-all duration-200 cursor-pointer",
    n > 0 && "mr-4",
    l ? "bg-primary-500/10 text-white" : "hover:bg-slate-800/50",
    t && "justify-center px-0"
  );
  return e.isExternal && e.path ? /* @__PURE__ */ p.jsx(
    "a",
    {
      href: e.path,
      target: "_blank",
      rel: "noopener noreferrer",
      className: f,
      children: d
    }
  ) : e.path && !c ? /* @__PURE__ */ p.jsx(mt, { to: e.path, className: f, children: d }) : /* @__PURE__ */ p.jsxs("div", { children: [
    /* @__PURE__ */ p.jsx("div", { onClick: u, className: f, children: d }),
    /* @__PURE__ */ p.jsx(Ye, { children: c && i && !t && /* @__PURE__ */ p.jsx(
      Te.div,
      {
        initial: { height: 0, opacity: 0 },
        animate: { height: "auto", opacity: 1 },
        exit: { height: 0, opacity: 0 },
        className: "overflow-hidden",
        children: /* @__PURE__ */ p.jsx("div", { className: "mt-1 space-y-1", children: e.children.map((m) => /* @__PURE__ */ p.jsx(
          da,
          {
            item: m,
            collapsed: t,
            depth: n + 1
          },
          m.id
        )) })
      }
    ) })
  ] });
}
function df({
  children: e,
  logo: t,
  menuItems: n = [],
  bottomContent: s
}) {
  var f, m, g, v, T;
  const r = qt(), o = U(Ni), i = U(Vi), { user: a, logout: l } = ve(), c = () => {
    r(vl());
  }, u = () => {
    r(Tl(!1));
  }, d = se(
    "fixed top-0 right-0 h-full z-50 flex flex-col",
    "bg-slate-900 border-l border-slate-800",
    "transition-all duration-300 ease-in-out",
    o ? "w-20" : "w-64",
    // Mobile
    "translate-x-full lg:translate-x-0",
    i && "translate-x-0"
  );
  return /* @__PURE__ */ p.jsx(p.Fragment, { children: /* @__PURE__ */ p.jsxs("aside", { className: d, children: [
    /* @__PURE__ */ p.jsxs("div", { className: se(
      "flex items-center h-16 px-4 border-b border-slate-800",
      o ? "justify-center" : "justify-between"
    ), children: [
      t || /* @__PURE__ */ p.jsxs("div", { className: "flex items-center gap-3", children: [
        /* @__PURE__ */ p.jsx("div", { className: "w-8 h-8 rounded-lg bg-gradient-to-br from-primary-500 to-neo-500 flex items-center justify-center text-white font-bold", children: "N" }),
        !o && /* @__PURE__ */ p.jsx(
          Te.span,
          {
            initial: { opacity: 0 },
            animate: { opacity: 1 },
            className: "text-lg font-bold text-white",
            children: "Neo BPMS"
          }
        )
      ] }),
      /* @__PURE__ */ p.jsx(
        "button",
        {
          onClick: c,
          className: se(
            "hidden lg:flex items-center justify-center w-8 h-8 rounded-lg",
            "text-slate-400 hover:text-white hover:bg-slate-800 transition-colors",
            o && "absolute -left-3 top-6 bg-slate-800 border border-slate-700"
          ),
          children: /* @__PURE__ */ p.jsx(
            Te.svg,
            {
              animate: { rotate: o ? 180 : 0 },
              className: "w-4 h-4",
              fill: "none",
              viewBox: "0 0 24 24",
              stroke: "currentColor",
              children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M15 19l-7-7 7-7" })
            }
          )
        }
      ),
      /* @__PURE__ */ p.jsx(
        "button",
        {
          onClick: u,
          className: "lg:hidden flex items-center justify-center w-8 h-8 rounded-lg text-slate-400 hover:text-white hover:bg-slate-800",
          children: /* @__PURE__ */ p.jsx("svg", { className: "w-5 h-5", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M6 18L18 6M6 6l12 12" }) })
        }
      )
    ] }),
    /* @__PURE__ */ p.jsxs("nav", { className: "flex-1 px-3 py-4 overflow-y-auto custom-scrollbar", children: [
      /* @__PURE__ */ p.jsx("div", { className: "space-y-1", children: n.map((x) => /* @__PURE__ */ p.jsx(da, { item: x, collapsed: o }, x.id)) }),
      e
    ] }),
    a && /* @__PURE__ */ p.jsx("div", { className: se(
      "p-4 border-t border-slate-800",
      o && "flex justify-center"
    ), children: o ? /* @__PURE__ */ p.jsx("div", { className: "w-10 h-10 rounded-full bg-gradient-to-br from-primary-500 to-neo-500 flex items-center justify-center text-white font-bold", children: ((f = a.displayName) == null ? void 0 : f.charAt(0)) || ((m = a.username) == null ? void 0 : m.charAt(0)) || "U" }) : /* @__PURE__ */ p.jsxs("div", { className: "flex items-center gap-3", children: [
      /* @__PURE__ */ p.jsx("div", { className: "w-10 h-10 rounded-full bg-gradient-to-br from-primary-500 to-neo-500 flex items-center justify-center text-white font-bold", children: ((g = a.displayName) == null ? void 0 : g.charAt(0)) || ((v = a.username) == null ? void 0 : v.charAt(0)) || "U" }),
      /* @__PURE__ */ p.jsxs("div", { className: "flex-1 min-w-0", children: [
        /* @__PURE__ */ p.jsx("p", { className: "text-sm font-medium text-white truncate", children: a.displayName || a.username }),
        /* @__PURE__ */ p.jsx("p", { className: "text-xs text-slate-400 truncate", children: a.email || ((T = a.roles) == null ? void 0 : T[0]) || "User" })
      ] }),
      /* @__PURE__ */ p.jsx(
        "button",
        {
          onClick: () => l(),
          className: "p-2 text-slate-400 hover:text-white hover:bg-slate-800 rounded-lg transition-colors",
          title: "خروج",
          children: /* @__PURE__ */ p.jsx("svg", { className: "w-5 h-5", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" }) })
        }
      )
    ] }) }),
    s && /* @__PURE__ */ p.jsx("div", { className: "p-4 border-t border-slate-800", children: s })
  ] }) });
}
function hf() {
  const e = U(Sl);
  return e.length ? /* @__PURE__ */ p.jsxs("nav", { className: "flex items-center gap-2 text-sm", children: [
    /* @__PURE__ */ p.jsx(mt, { to: "/", className: "text-slate-400 hover:text-primary-500 transition-colors", children: /* @__PURE__ */ p.jsx("svg", { className: "w-4 h-4", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" }) }) }),
    e.map((t, n) => /* @__PURE__ */ p.jsxs(Yt.Fragment, { children: [
      /* @__PURE__ */ p.jsx("svg", { className: "w-4 h-4 text-slate-500", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M9 5l7 7-7 7" }) }),
      t.path ? /* @__PURE__ */ p.jsx(
        mt,
        {
          to: t.path,
          className: "text-slate-400 hover:text-primary-500 transition-colors",
          children: t.label
        }
      ) : /* @__PURE__ */ p.jsx("span", { className: "text-slate-300 font-medium", children: t.label })
    ] }, n))
  ] }) : null;
}
function ff() {
  const [e, t] = Ft(!1), n = qt(), s = U(El), r = s.length;
  return /* @__PURE__ */ p.jsxs("div", { className: "relative", children: [
    /* @__PURE__ */ p.jsxs(
      "button",
      {
        onClick: () => t(!e),
        className: se(
          "relative p-2 rounded-lg transition-colors",
          "text-slate-400 hover:text-white hover:bg-slate-800/50"
        ),
        children: [
          /* @__PURE__ */ p.jsx("svg", { className: "w-5 h-5", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" }) }),
          r > 0 && /* @__PURE__ */ p.jsx("span", { className: "absolute top-1 right-1 w-4 h-4 bg-red-500 rounded-full text-xs text-white flex items-center justify-center", children: r > 9 ? "9+" : r })
        ]
      }
    ),
    /* @__PURE__ */ p.jsx(Ye, { children: e && /* @__PURE__ */ p.jsxs(p.Fragment, { children: [
      /* @__PURE__ */ p.jsx(
        "div",
        {
          className: "fixed inset-0 z-40",
          onClick: () => t(!1)
        }
      ),
      /* @__PURE__ */ p.jsxs(
        Te.div,
        {
          initial: { opacity: 0, y: -10, scale: 0.95 },
          animate: { opacity: 1, y: 0, scale: 1 },
          exit: { opacity: 0, y: -10, scale: 0.95 },
          className: "absolute left-0 mt-2 w-80 bg-slate-800 rounded-xl shadow-xl border border-slate-700 z-50 overflow-hidden",
          children: [
            /* @__PURE__ */ p.jsx("div", { className: "p-4 border-b border-slate-700", children: /* @__PURE__ */ p.jsx("h3", { className: "font-semibold text-white", children: "اعلان‌ها" }) }),
            /* @__PURE__ */ p.jsx("div", { className: "max-h-80 overflow-y-auto", children: s.length === 0 ? /* @__PURE__ */ p.jsx("div", { className: "p-4 text-center text-slate-400", children: "اعلان جدیدی وجود ندارد" }) : s.map((o) => /* @__PURE__ */ p.jsxs(
              "div",
              {
                className: se(
                  "p-4 border-b border-slate-700/50 hover:bg-slate-700/50 transition-colors",
                  "flex items-start gap-3"
                ),
                children: [
                  /* @__PURE__ */ p.jsxs("div", { className: se(
                    "w-8 h-8 rounded-full flex items-center justify-center",
                    o.type === "success" && "bg-green-500/20 text-green-400",
                    o.type === "error" && "bg-red-500/20 text-red-400",
                    o.type === "warning" && "bg-yellow-500/20 text-yellow-400",
                    o.type === "info" && "bg-blue-500/20 text-blue-400"
                  ), children: [
                    o.type === "success" && "✓",
                    o.type === "error" && "✕",
                    o.type === "warning" && "!",
                    o.type === "info" && "i"
                  ] }),
                  /* @__PURE__ */ p.jsxs("div", { className: "flex-1 min-w-0", children: [
                    /* @__PURE__ */ p.jsx("p", { className: "text-sm font-medium text-white", children: o.title }),
                    o.message && /* @__PURE__ */ p.jsx("p", { className: "text-xs text-slate-400 mt-1", children: o.message })
                  ] }),
                  /* @__PURE__ */ p.jsx(
                    "button",
                    {
                      onClick: () => n(bl(o.id)),
                      className: "text-slate-500 hover:text-white",
                      children: /* @__PURE__ */ p.jsx("svg", { className: "w-4 h-4", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M6 18L18 6M6 6l12 12" }) })
                    }
                  )
                ]
              },
              o.id
            )) })
          ]
        }
      )
    ] }) })
  ] });
}
function pf() {
  var o, i;
  const [e, t] = Ft(!1), { user: n, logout: s, isAdmin: r } = ve();
  return n ? /* @__PURE__ */ p.jsxs("div", { className: "relative", children: [
    /* @__PURE__ */ p.jsxs(
      "button",
      {
        onClick: () => t(!e),
        className: "flex items-center gap-2 p-1 rounded-lg hover:bg-slate-800/50 transition-colors",
        children: [
          /* @__PURE__ */ p.jsx("div", { className: "w-8 h-8 rounded-full bg-gradient-to-br from-primary-500 to-neo-500 flex items-center justify-center text-white text-sm font-bold", children: ((o = n.displayName) == null ? void 0 : o.charAt(0)) || ((i = n.username) == null ? void 0 : i.charAt(0)) || "U" }),
          /* @__PURE__ */ p.jsx("span", { className: "hidden md:block text-sm text-slate-300", children: n.displayName || n.username }),
          /* @__PURE__ */ p.jsx("svg", { className: "w-4 h-4 text-slate-400", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M19 9l-7 7-7-7" }) })
        ]
      }
    ),
    /* @__PURE__ */ p.jsx(Ye, { children: e && /* @__PURE__ */ p.jsxs(p.Fragment, { children: [
      /* @__PURE__ */ p.jsx("div", { className: "fixed inset-0 z-40", onClick: () => t(!1) }),
      /* @__PURE__ */ p.jsxs(
        Te.div,
        {
          initial: { opacity: 0, y: -10, scale: 0.95 },
          animate: { opacity: 1, y: 0, scale: 1 },
          exit: { opacity: 0, y: -10, scale: 0.95 },
          className: "absolute left-0 mt-2 w-56 bg-slate-800 rounded-xl shadow-xl border border-slate-700 z-50 overflow-hidden",
          children: [
            /* @__PURE__ */ p.jsxs("div", { className: "p-4 border-b border-slate-700", children: [
              /* @__PURE__ */ p.jsx("p", { className: "font-medium text-white", children: n.displayName || n.username }),
              /* @__PURE__ */ p.jsx("p", { className: "text-sm text-slate-400", children: n.email }),
              r && /* @__PURE__ */ p.jsx("span", { className: "inline-block mt-2 px-2 py-0.5 bg-primary-500/20 text-primary-400 text-xs rounded-full", children: "مدیر سیستم" })
            ] }),
            /* @__PURE__ */ p.jsxs("div", { className: "py-2", children: [
              /* @__PURE__ */ p.jsxs(
                mt,
                {
                  to: "/profile",
                  className: "flex items-center gap-3 px-4 py-2 text-sm text-slate-300 hover:bg-slate-700/50 transition-colors",
                  onClick: () => t(!1),
                  children: [
                    /* @__PURE__ */ p.jsx("svg", { className: "w-4 h-4", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" }) }),
                    "پروفایل"
                  ]
                }
              ),
              /* @__PURE__ */ p.jsxs(
                mt,
                {
                  to: "/settings",
                  className: "flex items-center gap-3 px-4 py-2 text-sm text-slate-300 hover:bg-slate-700/50 transition-colors",
                  onClick: () => t(!1),
                  children: [
                    /* @__PURE__ */ p.jsxs("svg", { className: "w-4 h-4", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: [
                      /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" }),
                      /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M15 12a3 3 0 11-6 0 3 3 0 016 0z" })
                    ] }),
                    "تنظیمات"
                  ]
                }
              )
            ] }),
            /* @__PURE__ */ p.jsx("div", { className: "py-2 border-t border-slate-700", children: /* @__PURE__ */ p.jsxs(
              "button",
              {
                onClick: () => {
                  t(!1), s();
                },
                className: "flex items-center gap-3 w-full px-4 py-2 text-sm text-red-400 hover:bg-slate-700/50 transition-colors",
                children: [
                  /* @__PURE__ */ p.jsx("svg", { className: "w-4 h-4", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" }) }),
                  "خروج از حساب"
                ]
              }
            ) })
          ]
        }
      )
    ] }) })
  ] }) : null;
}
function mf({
  children: e,
  showSearch: t = !0,
  showNotifications: n = !0,
  showThemeToggle: s = !0,
  customActions: r
}) {
  const o = qt(), i = U(ki), a = U(wl), l = () => {
    o(xl());
  }, c = () => {
    o(yl());
  };
  return /* @__PURE__ */ p.jsx("header", { className: "sticky top-0 z-30 bg-slate-900/80 backdrop-blur-lg border-b border-slate-800", children: /* @__PURE__ */ p.jsxs("div", { className: "flex items-center justify-between h-16 px-4 md:px-6", children: [
    /* @__PURE__ */ p.jsxs("div", { className: "flex items-center gap-4", children: [
      /* @__PURE__ */ p.jsx(
        "button",
        {
          onClick: l,
          className: "lg:hidden p-2 text-slate-400 hover:text-white hover:bg-slate-800/50 rounded-lg transition-colors",
          children: /* @__PURE__ */ p.jsx("svg", { className: "w-6 h-6", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M4 6h16M4 12h16M4 18h16" }) })
        }
      ),
      a && /* @__PURE__ */ p.jsx("h1", { className: "text-lg font-semibold text-white hidden md:block", children: a }),
      /* @__PURE__ */ p.jsx(hf, {})
    ] }),
    /* @__PURE__ */ p.jsxs("div", { className: "flex items-center gap-2", children: [
      t && /* @__PURE__ */ p.jsx("button", { className: "p-2 text-slate-400 hover:text-white hover:bg-slate-800/50 rounded-lg transition-colors", children: /* @__PURE__ */ p.jsx("svg", { className: "w-5 h-5", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" }) }) }),
      s && /* @__PURE__ */ p.jsx(
        "button",
        {
          onClick: c,
          className: "p-2 text-slate-400 hover:text-white hover:bg-slate-800/50 rounded-lg transition-colors",
          children: i.mode === "dark" ? /* @__PURE__ */ p.jsx("svg", { className: "w-5 h-5", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" }) }) : /* @__PURE__ */ p.jsx("svg", { className: "w-5 h-5", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ p.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" }) })
        }
      ),
      n && /* @__PURE__ */ p.jsx(ff, {}),
      r,
      /* @__PURE__ */ p.jsx(pf, {}),
      e
    ] })
  ] }) });
}
const gf = {
  hidden: { opacity: 0, y: 10 },
  visible: {
    opacity: 1,
    y: 0,
    transition: { duration: 0.2, ease: "easeOut" }
  },
  exit: {
    opacity: 0,
    y: -10,
    transition: { duration: 0.15 }
  }
}, yf = {
  hidden: { opacity: 0 },
  visible: { opacity: 1 }
};
function Cp({
  children: e,
  className: t,
  showHeader: n = !0,
  showSidebar: s = !0,
  headerContent: r,
  sidebarContent: o
}) {
  const i = U(Ni), a = U(Vi), l = U(ki);
  return /* @__PURE__ */ p.jsxs(
    "div",
    {
      className: se(
        "min-h-screen bg-slate-100 dark:bg-slate-900 transition-colors duration-300",
        l.direction === "rtl" ? "rtl" : "ltr",
        t
      ),
      dir: l.direction,
      children: [
        /* @__PURE__ */ p.jsx(Ye, { children: a && /* @__PURE__ */ p.jsx(
          Te.div,
          {
            variants: yf,
            initial: "hidden",
            animate: "visible",
            exit: "hidden",
            className: "fixed inset-0 bg-black/50 backdrop-blur-sm z-40 lg:hidden"
          }
        ) }),
        s && /* @__PURE__ */ p.jsx(df, { children: o }),
        /* @__PURE__ */ p.jsxs(
          "div",
          {
            className: se(
              "flex flex-col min-h-screen transition-all duration-300 ease-in-out",
              s && (i ? "lg:mr-20" : "lg:mr-64")
            ),
            children: [
              n && /* @__PURE__ */ p.jsx(mf, { children: r }),
              /* @__PURE__ */ p.jsx("main", { className: "flex-1 p-4 md:p-6 lg:p-8", children: /* @__PURE__ */ p.jsx(Ye, { mode: "wait", children: /* @__PURE__ */ p.jsx(
                Te.div,
                {
                  variants: gf,
                  initial: "hidden",
                  animate: "visible",
                  exit: "exit",
                  className: "h-full",
                  children: e || /* @__PURE__ */ p.jsx(Qa, {})
                },
                location.pathname
              ) }) }),
              /* @__PURE__ */ p.jsx("footer", { className: "px-6 py-4 border-t border-slate-200 dark:border-slate-800", children: /* @__PURE__ */ p.jsxs("div", { className: "flex items-center justify-between text-sm text-slate-500 dark:text-slate-400", children: [
                /* @__PURE__ */ p.jsxs("span", { children: [
                  "© ",
                  (/* @__PURE__ */ new Date()).getFullYear(),
                  " Neo BPMS"
                ] }),
                /* @__PURE__ */ p.jsx("span", { children: "Powered by Neo Platform" })
              ] }) })
            ]
          }
        )
      ]
    }
  );
}
export {
  Cp as AdminLayout,
  jl as ApiClient,
  Yf as AuthProvider,
  Ei as DEFAULT_API_CONFIG,
  ol as DEFAULT_AUTH_CONFIG,
  pn as ERROR_CODES,
  Y as EVENTS,
  Zs as HTTP_STATUS,
  mf as Header,
  Rf as PAGINATION_DEFAULTS,
  Ol as ProtectedRoute,
  Af as QUERY_KEYS,
  ip as RequireAdmin,
  sp as RequirePermission,
  rp as RequireRole,
  L as STORAGE_KEYS,
  df as Sidebar,
  If as addNotification,
  ne as apiClient,
  hl as authReducer,
  Sp as capitalize,
  dl as clearAuth,
  jf as clearAuthError,
  _f as clearNotifications,
  Rp as cn,
  Cl as createAppStore,
  pp as debounce,
  gp as deepClone,
  fp as delay,
  zf as dispatch,
  mn as fetchCurrentUser,
  dp as formatCurrency,
  hp as formatFileSize,
  up as formatNumber,
  vp as generateId,
  xp as get,
  Gf as getState,
  Bf as hideLoading,
  yp as isEmpty,
  ut as login,
  Ot as logout,
  bp as parseQueryString,
  Bt as refreshAccessToken,
  bl as removeNotification,
  Pi as selectAccessToken,
  Ri as selectAuthError,
  Ai as selectAuthIsLoading,
  Sl as selectBreadcrumbs,
  Ci as selectExpiresAt,
  Nf as selectHasPermission,
  Vf as selectHasRole,
  wi as selectIsAuthenticated,
  Kf as selectLanguage,
  Hf as selectLoadingMessage,
  El as selectNotifications,
  wl as selectPageTitle,
  Ni as selectSidebarCollapsed,
  Vi as selectSidebarMobileOpen,
  ki as selectTheme,
  Wf as selectUiIsLoading,
  Yn as selectUser,
  kf as sessionExpired,
  Df as setAuthError,
  Uf as setBreadcrumbs,
  Of as setLanguage,
  Tl as setMobileSidebarOpen,
  $f as setPageTitle,
  Lf as setSidebarCollapsed,
  Mf as setTheme,
  Cf as setTokens,
  Pf as setUser,
  Ff as showLoading,
  wp as slugify,
  Ap as storage,
  Xn as store,
  mp as throttle,
  Tp as toQueryString,
  xl as toggleMobileSidebar,
  vl as toggleSidebar,
  yl as toggleThemeMode,
  Ep as truncate,
  Al as uiReducer,
  ul as updateLastActivity,
  cl as updateUser,
  op as useApiMutation,
  Ll as useApiQuery,
  qt as useAppDispatch,
  U as useAppSelector,
  ve as useAuth,
  Nl as useAuthContext,
  Jf as useAuthError,
  Zf as useAuthLoading,
  ap as useCrudApi,
  qf as useCurrentUser,
  cp as useInfiniteApi,
  Xf as useIsAuthenticated,
  Il as usePaginatedQuery,
  Qf as usePermission,
  lp as usePrefetch,
  tp as useRequireAuth,
  ep as useRole,
  np as withProtectedRoute
};
//# sourceMappingURL=index.mjs.map
